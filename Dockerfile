# Usa un'immagine di base con .NET SDK


FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base 
WORKDIR /app
EXPOSE 80

# Usa l'immagine SDK per costruire il progetto
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Clinica.csproj", "./"]
RUN dotnet restore "Clinica.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Clinica.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Clinica.csproj" -c Release -o /app/publish

# Creazione dell'immagine finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clinica.dll"]


