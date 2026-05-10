# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["backend.csproj", "./"]
RUN dotnet restore "backend.csproj"

# Copiar o restante do código e publicar
COPY . .
RUN dotnet publish "backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final (Runtime/Migrations)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 1. Instalar a ferramenta dotnet-ef globalmente
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# 2. Instalar ferramentas necessárias para o entrypoint (wait-for-it pattern)
RUN apt-get update && apt-get install -y curl netcat-traditional && rm -rf /var/lib/apt/lists/*

# 3. Copiar os arquivos publicados do estágio de build
COPY --from=build /app/publish .

# 4. Copiar o código fonte raiz novamente e executar um novo dotnet restore
# Isso é obrigatório para que o 'dotnet ef database update' funcione no entrypoint (Erro NETSDK1064)
COPY . .
RUN dotnet restore

COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]
