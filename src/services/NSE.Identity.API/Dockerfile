﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/services/NSE.Identity.API/NSE.Identity.API.csproj", "src/services/NSE.Identity.API/"]
RUN dotnet restore "src/services/NSE.Identity.API/NSE.Identity.API.csproj"
COPY . .
WORKDIR "/src/src/services/NSE.Identity.API"
RUN dotnet build "NSE.Identity.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NSE.Identity.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NSE.Identity.API.dll"]
