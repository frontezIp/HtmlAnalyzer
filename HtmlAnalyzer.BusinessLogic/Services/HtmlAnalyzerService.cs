using System.Buffers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using FluentValidation;
using FluentResults;
using HtmlAnalyzer.DataAccess.Entities;
using HtmlAnalyzer.DataAccess.Repositories;
using DataElement = HtmlAnalyzer.DataAccess.Entities.Element;
using HtmlAnalyzer.BusinessLogic.Dtos;
using HtmlAnalyzer.BusinessLogic.Results;

namespace HtmlAnalyzer.BusinessLogic.Services;

public sealed class HtmlAnalyzerService(
    IValidator<AnalyzeHtmlRequestDto> validator,
    IElementRepository elementRepository) : IHtmlAnalyzerService
{
    private static readonly Encoding Utf8Strict = new UTF8Encoding(false, true);
    private static readonly Regex EmailRegex = new(
        @"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public async Task<Result<DecodedAnalyzeHtml>> ProcessAsync(AnalyzeHtmlRequestDto? request, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = ValidateRequest(request);

        if (validationResult.IsFailed)
        {
            return Result.Fail<DecodedAnalyzeHtml>(validationResult.Errors);
        }

        var urlResult = DecodeBase64Text(request!.UrlB64!, "url_b64");

        if (urlResult.IsFailed)
        {
            return Result.Fail<DecodedAnalyzeHtml>(urlResult.Errors);
        }

        var pageResult = DecodeBase64Text(request.PageB64!, "page_b64");

        if (pageResult.IsFailed)
        {
            return Result.Fail<DecodedAnalyzeHtml>(pageResult.Errors);
        }

        var encryptedTextResult = DecodeBase64Bytes(
            request.EncryptedTextBytesB64!,
            "encrypted_text_bytes_b64");

        if (encryptedTextResult.IsFailed)
        {
            return Result.Fail<DecodedAnalyzeHtml>(encryptedTextResult.Errors);
        }

        var keyResult = DecodeBase64Bytes(request.KeyBytesB64!, "key_bytes_b64");

        if (keyResult.IsFailed)
        {
            return Result.Fail<DecodedAnalyzeHtml>(keyResult.Errors);
        }

        var decryptedTextResult = DecryptText(encryptedTextResult.Value, keyResult.Value);

        if (decryptedTextResult.IsFailed)
        {
            return Result.Fail<DecodedAnalyzeHtml>(decryptedTextResult.Errors);
        }

        var document = ParsePage(pageResult.Value);
        IHtmlCollection<IElement> selectedElements;
        
        try
        {
            selectedElements = document.QuerySelectorAll(request.Selector!);
        }
        catch (DomException)
        {
            return Result.Fail<DecodedAnalyzeHtml>("The selector is not a valid CSS selector.");
        }

        var elementsAttrList = new List<string>(selectedElements.Length);

        foreach (var selectedElement in selectedElements)
        {
            var attributeValue = selectedElement.GetAttribute(request.Attribute!) ?? string.Empty;
            elementsAttrList.Add(attributeValue);
            await elementRepository.AddAsync(
                new DataElement
                {
                    Value = attributeValue,
                    HtmlCode = selectedElement.OuterHtml
                },
                cancellationToken);
        }

        var emailsList = FindEmailAddresses(pageResult.Value);

        return Result.Ok(new DecodedAnalyzeHtml(
            urlResult.Value,
            selectedElements.Length,
            emailsList.Count,
            elementsAttrList,
            emailsList,
            decryptedTextResult.Value));
    }

    private Result ValidateRequest(AnalyzeHtmlRequestDto? request)
    {
        if (request is null)
        {
            return Result.Fail("The request body is required.");
        }

        var validation = validator.Validate(request);

        return validation.IsValid
            ? Result.Ok()
            : Result.Fail(validation.Errors.Select(error => error.ErrorMessage));
    }

    private static IDocument ParsePage(string page)
    {
        var parser = new HtmlParser();

        return parser.ParseDocument(page);
    }

    private static List<string> FindEmailAddresses(string page)
    {
        return EmailRegex.Matches(page)
            .Select(match => match.Value)
            .ToList();
    }

    private static Result<string> DecodeBase64Text(string value, string fieldName)
    {
        var bytesResult = DecodeBase64Bytes(value, fieldName);
        if (bytesResult.IsFailed)
        {
            return Result.Fail<string>(bytesResult.Errors);
        }

        return DecodeUtf8Text(bytesResult.Value, fieldName);
    }

    private static Result<byte[]> DecodeBase64Bytes(string value, string fieldName)
    {
        var bytes = new byte[value.Length];
        if (!Convert.TryFromBase64String(value, bytes, out var bytesWritten))
        {
            return Result.Fail<byte[]>($"Field '{fieldName}' is not valid Base64.");
        }

        return Result.Ok(bytes[..bytesWritten]);
    }

    private static Result<string> DecodeUtf8Text(byte[] bytes, string fieldName)
    {
        var decodedBytes = bytes.AsSpan();
        var offset = 0;

        while (offset < decodedBytes.Length)
        {
            var status = Rune.DecodeFromUtf8(decodedBytes[offset..], out _, out var bytesConsumed);

            if (status != OperationStatus.Done)
            {
                return Result.Fail<string>($"Field '{fieldName}' does not contain valid UTF-8 text.");
            }

            offset += bytesConsumed;
        }

        return Result.Ok(Utf8Strict.GetString(decodedBytes));
    }

    private static Result<string> DecryptText(byte[] encryptedText, byte[] key)
    {
        if (key.Length != 32)
        {
            return Result.Fail<string>("The AES key must be 32 bytes long.");
        }

        if (encryptedText.Length == 0 || encryptedText.Length % 16 != 0)
        {
            return Result.Fail<string>("The encrypted text must contain complete AES blocks.");
        }

        try
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            using var decryptor = aes.CreateDecryptor();
            var plainText = decryptor.TransformFinalBlock(encryptedText, 0, encryptedText.Length);

            return DecodeUtf8Text(plainText, "decrypted_text");
        }
        catch (CryptographicException)
        {
            return Result.Fail<string>("The encrypted text could not be decrypted with the supplied AES key.");
        }
    }
}
