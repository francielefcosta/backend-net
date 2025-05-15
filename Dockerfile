# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /app

# Copia o csproj separado para cache otimizado
COPY ./backend.csproj ./
RUN dotnet restore "./backend.csproj"

# Agora copia todo o resto do projeto
COPY . ./
RUN dotnet publish "./backend.csproj" -c Release -o out

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "backend.dll"]
