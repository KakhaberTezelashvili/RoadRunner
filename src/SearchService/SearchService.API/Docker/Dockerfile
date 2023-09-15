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
COPY ["src/SearchService/SearchService.API/Docker/nuget.config", "."]
#RUN dotnet nuget update source Test --valid-authentication-types basic --username ${USERNAME} --password ${PAT} --store-password-in-clear-text

COPY ["src/SearchService/SearchService.API/SearchService.API.csproj", "SearchService/SearchService.API/"]
COPY ["src/SearchService/SearchService.Shared/SearchService.Shared.csproj", "SearchService/SearchService.Shared/"]
COPY ["src/SearchService/SearchService.Infrastructure/SearchService.Infrastructure.csproj", "SearchService/SearchService.Infrastructure/"]
COPY ["src/SearchService/SearchService.Core/SearchService.Core.csproj", "SearchService/SearchService.Core/"]
COPY ["src/Common/TDOC.Common/TDOC.Common.csproj", "Common/TDOC.Common/"]
COPY ["src/Common/TDOC.Common.Data/TDOC.Common.Data.csproj", "Common/TDOC.Common.Data/"]
COPY ["src/Common/TDOC.Common.Server/TDOC.Common.Server.csproj", "Common/TDOC.Common.Server/"]
COPY ["src/Domain/TDOC.Data/TDOC.Data.csproj", "Domain/TDOC.Data/"]
COPY ["src/Domain/TDOC.EntityFramework/TDOC.EntityFramework.csproj", "Domain/TDOC.EntityFramework/"]

RUN dotnet restore "SearchService/SearchService.API/SearchService.API.csproj"
COPY . .
WORKDIR "src/SearchService/SearchService.API"
RUN dotnet build "SearchService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SearchService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SearchService.API.dll"]