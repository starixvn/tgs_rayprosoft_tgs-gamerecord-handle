#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.Handle/Tgs.GameRecord.Handle.csproj", "Tgs.GameRecord.Handle/"]
COPY ["Tgs.GameRecord.Handle.Common/Tgs.GameRecord.Handle.Common.csproj", "Tgs.GameRecord.Handle.Common/"]
COPY ["Tgs.GameRecord.Handle.Core/Tgs.GameRecord.Handle.Core.csproj", "Tgs.GameRecord.Handle.Core/"]
COPY ["Tgs.GameRecord.Handle.Repositories/Tgs.GameRecord.Handle.Repositories.csproj", "Tgs.GameRecord.Handle.Repositories/"]
RUN dotnet restore "./Tgs.GameRecord.Handle/./Tgs.GameRecord.Handle.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.Handle"
RUN dotnet build "./Tgs.GameRecord.Handle.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.Handle.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.Handle.dll"]