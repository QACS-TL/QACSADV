#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
ENV DOTNET_URLS=http://+:3011
EXPOSE 3011

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BuyerService/BuyerService.csproj", "BuyerService/"]
RUN dotnet restore "./BuyerService/BuyerService.csproj"
COPY . .
WORKDIR "/src/BuyerService"
RUN dotnet build "./BuyerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BuyerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BuyerService.dll"]