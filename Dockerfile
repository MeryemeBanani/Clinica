FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Clinica/Clinica.csproj", "Clinica/"]
RUN dotnet restore "Clinica/Clinica.csproj"
COPY . .
WORKDIR "/src/Clinica"
RUN dotnet build "Clinica.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Clinica.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clinica.dll"]
