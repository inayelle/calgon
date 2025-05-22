FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Calgon.Host/Calgon.Host.csproj", "Calgon.Host/"]
RUN dotnet restore "Calgon.Host/Calgon.Host.csproj"
COPY . .
WORKDIR "/src/Calgon.Host"
RUN dotnet build "./Calgon.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Calgon.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/Calgon.Host/Properties ./Properties
ENTRYPOINT ["dotnet", "Calgon.Host.dll"]
