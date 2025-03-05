FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/BonusSystem.Shared/BonusSystem.Shared.csproj", "src/BonusSystem.Shared/"]
COPY ["src/BonusSystem.Core/BonusSystem.Core.csproj", "src/BonusSystem.Core/"]
COPY ["src/BonusSystem.Infrastructure/BonusSystem.Infrastructure.csproj", "src/BonusSystem.Infrastructure/"]
COPY ["src/BonusSystem.Api/BonusSystem.Api.csproj", "src/BonusSystem.Api/"]

RUN dotnet restore "src/BonusSystem.Api/BonusSystem.Api.csproj"

# Copy all source code and build the application
COPY . .
WORKDIR "/src/src/BonusSystem.Api"
RUN dotnet build "BonusSystem.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BonusSystem.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BonusSystem.Api.dll"]
