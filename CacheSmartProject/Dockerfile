# -----------------------
# BASE IMAGE
# -----------------------
# Bu image ASP.NET 8 üçün runtime mühitidir və yalnız tətbiqin işləməsi üçün istifadə olunur.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# -----------------------
# BUILD IMAGE
# -----------------------
# Bu image SDK ilə gəlir və build üçün istifadə olunur.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Lazım olan bütün .csproj faylları köçürülür
COPY ["CacheSmartProject/CacheSmartProject.csproj", "CacheSmartProject/"]
COPY ["CacheSmartProject.Application/CacheSmartProject.Application.csproj", "CacheSmartProject.Application/"]
COPY ["CacheSmartProject.Domain/CacheSmartProject.Domain.csproj", "CacheSmartProject.Domain/"]
COPY ["CacheSmartProject.Infrastructure/CacheSmartProject.Infrastructure.csproj", "CacheSmartProject.Infrastructure/"]
COPY ["CacheSmartProject.Migrations/CacheSmartProject.Migrations.csproj", "CacheSmartProject.Migrations/"]
COPY ["CacheSmartProject.Persistance/CacheSmartProject.Persistance.csproj", "CacheSmartProject.Persistance/"]

# NuGet paketləri bərpa olunur
RUN dotnet restore "./CacheSmartProject/CacheSmartProject.csproj"

# Layihənin bütün faylları köçürülür
COPY . .

# İndi əsas layihə qovluğuna daxil oluruq və build edirik
WORKDIR "/src/CacheSmartProject"
RUN dotnet build "./CacheSmartProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -----------------------
# PUBLISH IMAGE
# -----------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# Proyekt publish olunur (useAppHost=false parametri Windows ilə bağlı bəzi uyğunsuzluqları aradan qaldırır)
RUN dotnet publish "./CacheSmartProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -----------------------
# FINAL IMAGE
# -----------------------
FROM base AS final
WORKDIR /app

# Əvvəlki mərhələdən publish olunmuş fayllar gətirilir
COPY --from=publish /app/publish .

# Əsas giriş nöqtəsi – tətbiqin başladılması
ENTRYPOINT ["dotnet", "CacheSmartProject.dll"]
