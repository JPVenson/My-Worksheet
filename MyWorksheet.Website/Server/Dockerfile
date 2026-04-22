#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# RUN:
# Increment Tag no:
# docker build -t ghcr.io/myworksheet:1.00 -f "MyWorksheet.Website/Server/Dockerfile" .
# Then if not logged in:
# docker image push ghcr.io/myworksheet:1.00
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# RUN pwsh bin\Debug\net6\playwright.ps1 install --with-deps chromium
 
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
RUN apt-get update && apt-get install -y --no-install-recommends python3 npm && rm -rf /var/lib/apt/lists/*
RUN npm install --global sass
RUN dotnet workload install wasm-tools --skip-manifest-update


WORKDIR /src
COPY ["MyWorksheet.Website/Server/MyWorksheet.Website.Server.csproj", "MyWorksheet.Website/Server/"]
COPY ["MyWorksheet.Website/Client/MyWorksheet.Website.Client.csproj", "MyWorksheet.Website/Client/"]
COPY ["MyWorksheet.Website/Shared/MyWorksheet.Website.Shared.csproj", "MyWorksheet.Website/Shared/"]


RUN dotnet restore "MyWorksheet.Website/Server/MyWorksheet.Website.Server.csproj"
COPY . .
RUN sass --no-source-map /src/MyWorksheet.Website/Client/wwwroot/css/preApp/pre-app.scss:/src/MyWorksheet.Website/Client/wwwroot/css/preApp/pre-app.css \
    && sass --style=compressed --no-source-map /src/MyWorksheet.Website/Client/wwwroot/css/preApp/pre-app.scss:/src/MyWorksheet.Website/Client/wwwroot/css/preApp/pre-app.min.css \
    && sass --no-source-map /src/MyWorksheet.Website/Client/wwwroot/css/themes/dark/site-theme.scss:/src/MyWorksheet.Website/Client/wwwroot/css/themes/dark/site-theme.css \
    && sass --style=compressed --no-source-map /src/MyWorksheet.Website/Client/wwwroot/css/themes/dark/site-theme.scss:/src/MyWorksheet.Website/Client/wwwroot/css/themes/dark/site-theme.min.css \
    && sass --no-source-map /src/MyWorksheet.Website/Client/wwwroot/css/themes/light/site-theme.scss:/src/MyWorksheet.Website/Client/wwwroot/css/themes/light/site-theme.css \
    && sass --style=compressed --no-source-map /src/MyWorksheet.Website/Client/wwwroot/css/themes/light/site-theme.scss:/src/MyWorksheet.Website/Client/wwwroot/css/themes/light/site-theme.min.css
WORKDIR "/src/MyWorksheet.Website/Server"
RUN dotnet build "MyWorksheet.Website.Server.csproj" -c Release -o /app/build --property WarningLevel=0

FROM build AS publish
RUN dotnet publish "MyWorksheet.Website.Server.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app
#RUN dotnet tool install --global Microsoft.Playwright.CLI
#RUN playwright install

COPY --from=publish /app/publish .


RUN cat /etc/issue & sleep 10

#Install powershell to playwright scripts
# RUN apt-get update -yq \
#     && apt-get install wget -yq \
#     && wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb \
#     && dpkg -i packages-microsoft-prod.deb \
#     && apt-get update -yq \
#     && apt-get install powershell -yq
# RUN dotnet tool update --global PowerShell
# RUN pwsh /app/build/playwright.ps1 install --with-deps chromium
#RUN pwsh /app/build/playwright.ps1 install-deps

# When changing the list of packages for playwright here also update the devcontainer!!!
RUN apt-get update && apt-get install -y --no-install-recommends \
    fonts-liberation \
    libasound2t64 \
    libatk-bridge2.0-0t64 \
    libatk1.0-0t64 \
    libatspi2.0-0t64 \
    libcairo2 \
    libcups2t64 \
    libdbus-1-3 \
    libdrm2 \
    libffi8 \
    libgbm1 \
    libglib2.0-0t64 \
    libgtk-3-0t64 \
    libicu74 \
    libnspr4 \
    libnss3 \
    libpango-1.0-0 \
    libx11-6 \
    libx264-164 \
    libxcb1 \
    libxcomposite1 \
    libxdamage1 \
    libxext6 \
    libxfixes3 \
    libxrandr2

ENTRYPOINT ["dotnet", "MyWorksheet.Website.Server.dll"]