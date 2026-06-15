# ==========================================
# ETAPA 1: Compilarea (SDK-ul greu)
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build   
WORKDIR /src

# 1. Copiem doar fișierul proiectului și descărcăm pachetele (NuGet)
COPY *.csproj ./
RUN dotnet restore

# 2. Copiem restul codului și îl compilăm pentru producție
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# ==========================================
# ETAPA 2: Rularea (Runtime-ul ușor)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime 
WORKDIR /app

# 3. Aducem doar codul gata compilat din Etapa 1
COPY --from=build /app/publish .

# 4. Spunem containerului ce să pornească
ENTRYPOINT ["dotnet", "ClientService.dll"]