#!/bin/bash
set -e

# Se a chave JWT não for fornecida, gera uma aleatória para evitar erro de startup
if [ -z "$Jwt__Key" ]; then
  echo "AVISO: Jwt__Key não definida. Gerando uma chave temporária com openssl..."
  export Jwt__Key=$(openssl rand -base64 32)
fi

# Aguarda o SQL Server ficar disponível
echo "Aguardando o SQL Server em $DB_HOST:1433..."
until nc -z $DB_HOST 1433; do
  echo "SQL Server ainda não está pronto. Dormindo 2 segundos..."
  sleep 2
done

echo "SQL Server está online! Verificando migrações..."

# Executa migrações
echo "Aplicando migrações ao banco de dados..."
dotnet ef database update --no-build

echo "Iniciando a aplicação..."
dotnet backend.dll
