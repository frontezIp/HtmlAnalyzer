FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /src

EXPOSE 8090

ENTRYPOINT ["dotnet", "run", "--project", "/src/HtmlAnalyzer/HtmlAnalyzer.API.csproj", "--urls", "http://0.0.0.0:8090"]
