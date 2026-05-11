-- Cria o banco de dados se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Vertrau')
BEGIN
    CREATE DATABASE Vertrau;
END
GO

USE Vertrau;
GO

-- Cria o Login no servidor se não existir
IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'dbuser_vertrau')
BEGIN
    CREATE LOGIN dbuser_vertrau WITH PASSWORD = '$(DB_PASSWORD)', CHECK_POLICY = OFF;
END
GO

-- Cria o Usuário no banco de dados vinculado ao Login
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'dbuser_vertrau')
BEGIN
    CREATE USER dbuser_vertrau FOR LOGIN dbuser_vertrau;
    -- Atribui permissão de db_owner apenas para este banco
    ALTER ROLE db_owner ADD MEMBER dbuser_vertrau;
END
GO
