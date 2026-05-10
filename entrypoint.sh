#!/bin/bash
set -e

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
