FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Yandex_Cinema/Yandex_Cinema.csproj", "Yandex_Cinema/"]
RUN dotnet restore "Yandex_Cinema/Yandex_Cinema.csproj"
COPY . .
WORKDIR "/src/Yandex_Cinema"
RUN dotnet build "./Yandex_Cinema.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Yandex_Cinema.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yandex_Cinema.dll"]
