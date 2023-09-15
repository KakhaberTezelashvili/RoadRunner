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
COPY ["src/AdminClient/AdminClient.WebApp/Docker/nuget.config", "."]
#RUN dotnet nuget update source Test --valid-authentication-types basic --username ${USERNAME} --password ${PAT} --store-password-in-clear-text
RUN dotnet nuget update source DevExpressFeed --valid-authentication-types basic --username ${DEUSERNAME} --password ${DEPAT} --store-password-in-clear-text

COPY ["src/AdminClient/AdminClient.WebApp/AdminClient.WebApp.csproj", "AdminClient/AdminClient.WebApp/"]
COPY ["src/MasterDataService/MasterDataService.Shared/MasterDataService.Shared.csproj", "MasterDataService/MasterDataService.Shared/"]
COPY ["src/TDOC.WebComponents/TDOC.WebComponents.csproj", "TDOC.WebComponents/"]
COPY ["src/TDOC.Resources/TDOC.Resources.csproj", "TDOC.Resources/"]
COPY ["src/Common/TDOC.Common.Client/TDOC.Common.Client.csproj", "Common/TDOC.Common.Client/"]
COPY ["build/webcompiler/InstallWebCompiler.sh", "/etc/webcompiler/"]
COPY ["build/webcompiler/CompileStaticAssets.sh", "/etc/webcompiler/"]
COPY ["build/webcompiler/MakeGeneratedDirectory.sh", "/etc/webcompiler/"]

RUN dotnet restore --interactive "AdminClient/AdminClient.WebApp/AdminClient.WebApp.csproj"
COPY . .
WORKDIR "src/AdminClient/AdminClient.WebApp"
RUN dotnet build "AdminClient.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdminClient.WebApp.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY ["build/nginx/nginx.conf", "/etc/nginx/nginx.conf"]
COPY ["build/nginx/proxy.conf", "/etc/nginx/proxy.conf"]
COPY ["build/ssl/certs/localhost.crt", "/etc/ssl/certs/localhost.crt"]
COPY ["build/ssl/private/localhost.key", "/etc/ssl/private/localhost.key"]