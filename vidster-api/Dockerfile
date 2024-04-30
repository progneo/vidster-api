FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY vesta-api/*.csproj vesta-api.csproj
RUN dotnet restore vesta-api.csproj
COPY vesta-api/. .
WORKDIR /src
RUN dotnet build vesta-api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish vesta-api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "vesta-api.dll"]