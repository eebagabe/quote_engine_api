# Quote Engine API

Rode em conjunto:

[Frontend](https://github.com/ebagabee/quote_engine) 

Passo a passo para rodar o projeto localmente.

## Dependências

- .NET SDK 8.0
- PostgreSQL 18.4
- dotnet-ef (`dotnet tool install --global dotnet-ef`)

## Configuração

### 1. Clonar o repositório

```bash
git clone git@github.com:ebagabee/quote_engine_api.git
cd quote_engine_api
```

### 2. Restaurar dependências

```bash
dotnet restore
```

### 3. Configurar o ambiente

***Crie um banco de dados localmente***
- Em seu gerenciador de banco de dados, ou internamente no terminal postgres:

```sql
CREATE DATABASE quote_engine
```

- Edite a connection string em `src/QuoteEngine.Api/appsettings.json` com suas credenciais do PostgreSQL:

```json
"ConnectionStrings": {
  "Default": "Host=localhost;Port=5432;Database=quote_engine;Username=postgres;Password=sua_senha"
}
```

Ou defina pela variável de ambiente `ConnectionStrings__Default`.

### 4. Rodar as migrations

```bash
dotnet ef database update -p src/QuoteEngine.Api
```

### 5. Subir o servidor

```bash
dotnet run --project src/QuoteEngine.Api
```

A API estará disponível em `http://localhost:8000`.

## Endpoints

- `POST /api/quotes`: calcula e salva uma cotação
- `GET /api/quotes`: lista todas as cotações

### Para Executar testes

```bash
dotnet test
```
