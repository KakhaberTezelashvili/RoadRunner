#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

#Installing Azure DevOps certificate (https://devops.gldk.t-docnet.com).
COPY ["build/ssl/certs/azure-gldk-onpremise.crt", "/usr/local/share/ca-certificates/extra/"]
RUN update-ca-certificates

#Updating Azure DevOps Artifacts repository in nuget.config using authentication data from command line arguments.
ARG PAT
ARG USERNAME
COPY ["src/MasterDataService/MasterDataService.API/Docker/nuget.config", "."]
#RUN dotnet nuget update source Test --valid-authentication-types basic --username ${USERNAME} --password ${PAT} --store-password-in-clear-text

COPY ["src/MasterDataService/MasterDataService.API/MasterDataService.API.csproj", "MasterDataService/MasterDataService.API/"]
COPY ["src/MasterDataService/MasterDataService.Shared/MasterDataService.Shared.csproj", "MasterDataService/MasterDataService.Shared/"]
COPY ["src/MasterDataService/MasterDataService.Infrastructure/MasterDataService.Infrastructure.csproj", "MasterDataService/MasterDataService.Infrastructure/"]
COPY ["src/MasterDataService/MasterDataService.Core/MasterDataService.Core.csproj", "MasterDataService/MasterDataService.Core/"]
COPY ["src/Common/TDOC.Common/TDOC.Common.csproj", "Common/TDOC.Common/"]
COPY ["src/Common/TDOC.Common.Data/TDOC.Common.Data.csproj", "Common/TDOC.Common.Data/"]
COPY ["src/Common/TDOC.Common.Server/TDOC.Common.Server.csproj", "Common/TDOC.Common.Server/"]
COPY ["src/Domain/TDOC.Data/TDOC.Data.csproj", "Domain/TDOC.Data/"]
COPY ["src/Domain/TDOC.EntityFramework/TDOC.EntityFramework.csproj", "Domain/TDOC.EntityFramework/"]

RUN dotnet restore --interactive "MasterDataService/MasterDataService.API/MasterDataService.API.csproj"
COPY . .
WORKDIR "src/MasterDataService/MasterDataService.API"
RUN dotnet build "MasterDataService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MasterDataService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MasterDataService.API.dll"]