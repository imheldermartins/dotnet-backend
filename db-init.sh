#!/bin/bash

# Inicia o SQL Server em segundo plano
/opt/mssql/bin/sqlservr &

# Aguarda o SQL Server iniciar (até 60 segundos)
echo "Aguardando o SQL Server para rodar scripts de configuração..."
for i in {1..60}; do
    # Tenta conectar com o sa para verificar se está pronto
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SELECT 1" > /dev/null 2>&1
    if [ $? -eq 0 ]; then
        echo "SQL Server pronto! Executando setup.sql..."
        /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /usr/config/setup.sql
        break
    fi
    echo "Ainda não está pronto... ($i/60)"
    sleep 2
done

# Mantém o processo do SQL Server em primeiro plano
wait
