#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Bps.uCloud.API/Bps.uCloud.API.csproj", "Bps.uCloud.API/"]
RUN dotnet restore "Bps.uCloud.API/Bps.uCloud.API.csproj"
COPY . .
WORKDIR "/src/Bps.uCloud.API"
RUN dotnet build "Bps.uCloud.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bps.uCloud.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bps.uCloud.API.dll"]