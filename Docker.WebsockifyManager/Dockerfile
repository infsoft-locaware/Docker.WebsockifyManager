# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update && apt-get install -y --no-install-recommends python3-pip python-is-python3 git && rm -rf /var/lib/apt/lists/* 
RUN git clone https://github.com/novnc/websockify.git /websockify
WORKDIR /websockify
RUN python setup.py install

USER $APP_UID
WORKDIR /app
EXPOSE 443

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Docker.WebsockifyManager/Docker.WebsockifyManager.csproj", "Docker.WebsockifyManager/"]
RUN dotnet restore "./Docker.WebsockifyManager.csproj"
COPY . .
WORKDIR "/src/Docker.WebsockifyManager"
RUN dotnet build "./Docker.WebsockifyManager.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Docker.WebsockifyManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Docker.WebsockifyManager.dll"]