# ============================
# STAGE 1: BUILD
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY AppChat.sln .
COPY AppChat/ ./AppChat/

RUN dotnet restore AppChat/AppChat.csproj
RUN dotnet publish AppChat/AppChat.csproj -c Release -o /app/publish

# ============================
# STAGE 2: RUNTIME
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

ENTRYPOINT ["dotnet", "AppChat.dll"]
