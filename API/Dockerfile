FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copier les fichiers de projet et restaurer les dépendances
COPY ["API.csproj", "."]
RUN dotnet restore "API.csproj"

# Copier tout le code source et publier l'application
COPY . .
WORKDIR "/source"
RUN dotnet publish -c Release -o /app/publish

# Étape de runtime (image finale plus légère)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Exposer le port (par défaut 80 pour HTTP, 443 pour HTTPS)
EXPOSE 80

# Point d'entrée
ENTRYPOINT ["dotnet", "API.dll"]