# Vertrau - Backend (.NET 10)

Este é o backend do projeto **Vertrau**, uma aplicação robusta desenvolvida com .NET 10, focada em segurança, escalabilidade e boas práticas de desenvolvimento. O sistema gerencia usuários e postagens, utilizando autenticação baseada em JWT com suporte a Refresh Tokens.

## 🚀 Tecnologias Utilizadas

- **Runtime:** .NET 10.0 (Web SDK)
- **Linguagem:** C# 13
- **Banco de Dados:** SQL Server 2025
- **ORM:** Entity Framework Core
- **Autenticação:** JWT Bearer com Refresh Tokens
- **Documentação:** Swagger/OpenAPI
- **Containerização:** Docker & Docker Compose

## 🏗️ Arquitetura e Padrões

O projeto segue princípios modernos de desenvolvimento .NET:
- **Expression-bodied members** e **Primary constructors** para código conciso.
- **Async First:** Todas as operações de I/O são assíncronas.
- **RESTful API:** Endpoints estruturados e padronizados via Controllers.
- **DTOs:** Separação clara entre modelos de domínio e objetos de transferência de dados.

## 🛠️ Configuração do Ambiente

### Pré-requisitos
- Docker e Docker Compose instalados.
- (Opcional) .NET 10 SDK para desenvolvimento local sem Docker.

### Execução via Docker (Recomendado)

O ambiente Docker está configurado para ser "zero-config". O banco de dados e as migrações são gerenciados automaticamente.

1. Suba os containers:
   ```bash
   docker compose up -d --build
   ```

2. O `entrypoint.sh` fará o seguinte automaticamente:
   - Aguardará o SQL Server ficar online.
   - Gerará uma chave JWT aleatória (caso não seja fornecida).
   - **Criará a migração inicial (`InitialSchema`)** se a pasta `Migrations` não existir.
   - Aplicará as migrações ao banco de dados.

3. Acesse a documentação da API (Swagger):
   - `http://localhost:5171/swagger`

### Execução Local (Bare Metal)

1. Configure sua Connection String no `appsettings.Development.json` ou via User Secrets.
2. Configure a chave JWT local:
   ```bash
   dotnet user-secrets set "Jwt:Key" "sua-chave-secreta-de-pelo-menos-32-caracteres"
   ```
3. Execute as migrações:
   ```bash
   dotnet ef database update
   ```
4. Inicie a aplicação:
   ```bash
   dotnet run
   ```

## 🔒 Segurança (JWT)

A aplicação utiliza autenticação JWT.
- No Docker, a chave é gerada dinamicamente via `openssl` no `entrypoint.sh` se a variável `JWT_KEY` estiver vazia.
- Em produção, defina a variável de ambiente `Jwt__Key`.

## 📂 Estrutura de Pastas

- `Controllers/`: Endpoints da API (`Auth`, `Posts`, `Users`).
- `Data/`: Contexto do EF Core (`AppDbContext`).
- `Entities/`: Modelos de domínio.
- `Dtos/`: Objetos de transferência de dados para requisições e respostas.
- `Properties/`: Configurações de lançamento e ambiente.

## 📝 Scripts Úteis

- **Derrubar ambiente Docker:** `docker compose down -v`
- **Verificar logs:** `docker logs -f learning-dotnet-api`
- **Criar nova migração:** `dotnet ef migrations add <Nome>`

---
Desenvolvido como parte do aprendizado de ecossistema .NET e Angular.
