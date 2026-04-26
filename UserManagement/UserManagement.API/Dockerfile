FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#this one is the user that we will use in the machine 
USER app
WORKDIR /app
# here we  mention the ports that the machine will listen to 
EXPOSE 8080
EXPOSE 8081

# the second layer 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# arg is used  to create a variable that can be used later 
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

#this command is used to copy the csproj file from my local machine project into the docker image folder named usermangement .api 
COPY ["UserManagement.API/UserManagement.API.csproj", "UserManagement.API/"]
RUN dotnet restore "./UserManagement.API/UserManagement.API.csproj"

# Copy everything else
# What it does: Copies all files from your current local directory (where the Dockerfile is)
#  into the current working directory inside the container (/src).
# so the copy is mainly used to copy from the local project into the docker machine or the docker container 

COPY . .
RUN dotnet build "./UserManagement.API/UserManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserManagement.API/UserManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManagement.API.dll"]




