FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProductManagement.API/ProductManagement.API.csproj", "ProductManagement.API/"]
RUN dotnet restore "./ProductManagement.API/ProductManagement.API.csproj"
COPY . .
RUN dotnet build "./ProductManagement.API/ProductManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductManagement.API/ProductManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV MYSQL_HOST=localhost
ENV MYSQL_PASSWORD=admin
ENV PortNumber=3306

ENTRYPOINT ["dotnet", "ProductManagement.API.dll"]