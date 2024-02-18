# Usa a imagem oficial do SDK do .NET 7 para construir a aplicação
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copia os arquivos do projeto e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia todos os arquivos e constrói a aplicação
COPY . ./
RUN dotnet publish -c Release -o out

# Cria uma imagem para a aplicação final
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Rinha.Backend.API.dll"]