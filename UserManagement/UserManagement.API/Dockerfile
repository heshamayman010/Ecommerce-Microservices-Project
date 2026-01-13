FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 9090
EXPOSE 9091

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["UserManagement.API/UserManagement.API.csproj", "UserManagement.API/"]
RUN dotnet restore "./UserManagement.API/UserManagement.API.csproj"

# Copy everything else
COPY . .
RUN dotnet build "./UserManagement.API/UserManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserManagement.API/UserManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV MyPostGrHost=postgres
ENV MyPostGrPassword=admin
ENV MyPostGrPort=5432 

ENTRYPOINT ["dotnet", "UserManagement.API.dll"]
