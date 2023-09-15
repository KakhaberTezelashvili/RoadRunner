#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

#Installing Azure DevOps certificate (https://devops.gldk.t-docnet.com).
COPY ["build/ssl/certs/azure-gldk-onpremise.crt", "/usr/local/share/ca-certificates/extra/"]
RUN update-ca-certificates

#Updating Azure DevOps Artifacts repository in nuget.config using authentication data from command line arguments.
ARG PAT
ARG USERNAME
ARG DEPAT
ARG DEUSERNAME
COPY ["src/ScannerClient/ScannerClient.WebApp/Docker/nuget.config", "."]
#RUN dotnet nuget update source Test --valid-authentication-types basic --username ${USERNAME} --password ${PAT} --store-password-in-clear-text
RUN dotnet nuget update source DevExpressFeed --valid-authentication-types basic --username ${DEUSERNAME} --password ${DEPAT} --store-password-in-clear-text

COPY ["src/ScannerClient/ScannerClient.WebApp/ScannerClient.WebApp.csproj", "ScannerClient/ScannerClient.WebApp/"]
COPY ["src/ProductionService/ProductionService.Shared/ProductionService.Shared.csproj", "ProductionService/ProductionService.Shared/"]
COPY ["src/SearchService/SearchService.Shared/SearchService.Shared.csproj", "SearchService/SearchService.Shared/"]
COPY ["src/TDOC.WebComponents/TDOC.WebComponents.csproj", "TDOC.WebComponents/"]
COPY ["src/TDOC.Resources/TDOC.Resources.csproj", "TDOC.Resources/"]
COPY ["src/Common/TDOC.Common/TDOC.Common.csproj", "Common/TDOC.Common/"]
COPY ["src/Common/TDOC.Common.Data/TDOC.Common.Data.csproj", "Common/TDOC.Common.Data/"]
COPY ["src/Common/TDOC.Common.Client/TDOC.Common.Client.csproj", "Common/TDOC.Common.Client/"]
COPY ["src/Domain/TDOC.Data/TDOC.Data.csproj", "Domain/TDOC.Data/"]
COPY ["src/Domain/TDOC.ModelToDbMapper/TDOC.ModelToDbMapper.csproj", "Domain/TDOC.ModelToDbMapper/"]
COPY ["build/webcompiler/InstallWebCompiler.sh", "/etc/webcompiler/"]
COPY ["build/webcompiler/CompileStaticAssets.sh", "/etc/webcompiler/"]
COPY ["build/webcompiler/MakeGeneratedDirectory.sh", "/etc/webcompiler/"]

RUN dotnet restore --interactive "ScannerClient/ScannerClient.WebApp/ScannerClient.WebApp.csproj"
COPY . .
WORKDIR "src/ScannerClient/ScannerClient.WebApp"
RUN dotnet build "ScannerClient.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScannerClient.WebApp.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY ["build/nginx/nginx.conf", "/etc/nginx/nginx.conf"]
COPY ["build/nginx/proxy.conf", "/etc/nginx/proxy.conf"]
COPY ["build/ssl/certs/localhost.crt", "/etc/ssl/certs/localhost.crt"]
COPY ["build/ssl/private/localhost.key", "/etc/ssl/private/localhost.key"]