# Use a imagem oficial do .NET SDK para build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copia os arquivos do projeto e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante do código e builda a aplicação
COPY . ./
RUN dotnet publish -c Release -o out

# Usa a imagem do runtime para rodar a aplicação (mais leve)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Porta que seu backend vai expor
EXPOSE 5007

# Comando para rodar a aplicação
ENTRYPOINT ["dotnet", "backend.dll"] 
