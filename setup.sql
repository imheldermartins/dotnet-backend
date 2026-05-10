-- Cria o banco de dados se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DbTeste')
BEGIN
    CREATE DATABASE DbTeste;
END
GO

USE DbTeste;
GO

-- Cria o Login no servidor se não existir
IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'dbuser_testedb')
BEGIN
    CREATE LOGIN dbuser_testedb WITH PASSWORD = 'passDbtestesupreme!', CHECK_POLICY = OFF;
END
GO

-- Cria o Usuário no banco de dados vinculado ao Login
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'dbuser_testedb')
BEGIN
    CREATE USER dbuser_testedb FOR LOGIN dbuser_testedb;
    -- Atribui permissão de db_owner apenas para este banco
    ALTER ROLE db_owner ADD MEMBER dbuser_testedb;
END
GO
