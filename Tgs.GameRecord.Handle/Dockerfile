#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update \
	&& apt-get install -y wget \

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.Handle/Tgs.GameRecord.Handle.csproj", "Tgs.GameRecord.Handle/"]
COPY ["Tgs.GameRecord.Handle.Common/Tgs.GameRecord.Handle.Common.csproj", "Tgs.GameRecord.Handle.Common/"]
COPY ["Tgs.GameRecord.Handle.Core/Tgs.GameRecord.Handle.Core.csproj", "Tgs.GameRecord.Handle.Core/"]
COPY ["Tgs.GameRecord.Handle.Repositories/Tgs.GameRecord.Handle.Repositories.csproj", "Tgs.GameRecord.Handle.Repositories/"]
RUN dotnet restore "Tgs.GameRecord.Handle/Tgs.GameRecord.Handle.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.Handle"
RUN dotnet build "Tgs.GameRecord.Handle.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.Handle.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.Handle.dll"]