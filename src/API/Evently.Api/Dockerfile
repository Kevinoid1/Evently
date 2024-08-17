FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

#build context is very important. It is the directory sent to the docker-daemon. docker -daemon can only access files in this directory

#setting build context as . means the current directory from where the docker file or docker-compose is being run

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# setting workdir means setting the directory in the container
COPY ["src/API/Evently.Api/Evently.Api.csproj", "src/API/Evently.Api/"]
RUN dotnet restore "./src/API/Evently.Api/Evently.Api.csproj"
COPY . .
WORKDIR "/src/src/API/Evently.Api"
RUN dotnet build "./Evently.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Evently.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Evently.Api.dll"]