#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-sac2016 AS base
WORKDIR /app
EXPOSE 53318
EXPOSE 44391

FROM microsoft/dotnet:2.1-sdk-nanoserver-sac2016 AS build
WORKDIR /src
COPY ["Bps.uCloud.Gateway/Bps.uCloud.Gateway.csproj", "Bps.uCloud.Gateway/"]
RUN dotnet restore "Bps.uCloud.Gateway/Bps.uCloud.Gateway.csproj"
COPY . .
WORKDIR "/src/Bps.uCloud.Gateway"
RUN dotnet build "Bps.uCloud.Gateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Bps.uCloud.Gateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Bps.uCloud.Gateway.dll"]