# Bookstore Backend

.NET 8 backend for a bookstore: Book CRUD (OAuth2 client-credentials flow) and
paginated book search (OAuth2 implicit flow, restricted to the `ProCredit`
claimed group).

## Prerequisites

- .NET 8 SDK
- Docker Desktop

## Running the full stack

Create a `.env` file at the repo root (gitignored — never committed) with:

```
DB_USER=sa
SA_PASSWORD=<pick your own local dev password>
PUBLIC_IDENTITYSERVER_URL=http://localhost:5001
PUBLIC_API_URL=http://localhost:5000
```

The two `PUBLIC_*` values are whatever address a **browser** can actually
reach IdentityServer/the API at — `localhost` for local dev, but on a real
deployment they must be the real host/IP (e.g.
`http://203.0.113.10:5001`/`http://203.0.113.10:5000`), never an internal
Docker service name. They control Swagger's OAuth2 URLs and IdentityServer's
issuer/redirect-URI, all of which are read by the browser or checked against
what the browser sends — a value only reachable from inside the Docker
network breaks the login flow.

Then:

```bash
docker compose up --build
```

Once all three containers are up:
- Swagger UI: http://localhost:5000/swagger
- IdentityServer discovery document: http://localhost:5001/.well-known/openid-configuration

### Test credentials

| Purpose | Client ID | Secret | Scope |
|---|---|---|---|
| CRUD (client credentials) | `bookstore.crud.client` | `dev-secret` | `bookstore.crud` |
| Search (implicit) | `bookstore.search.client` | (none — public client) | `bookstore.search` |

| Username | Password | `group` claim | Can use search? |
|---|---|---|---|
| `alice` | `Pass123$` | `ProCredit` | Yes |
| `bob` | `Pass123$` | (none) | No — `403 Forbidden` |

In Swagger's **Authorize** dialog, enter the relevant client ID (and secret,
for the CRUD flow) into its corresponding flow section, then check the scope
and authorize.

## Running the API without Docker

`appsettings.json` never contains DB credentials — the API reads them from the
`.env` file described above, the same one the Docker Compose stack uses. Make
sure it exists, start a SQL Server container on `localhost:1433` (e.g.
`docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<value from .env>" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest`
if you need one), then:

```bash
dotnet run --project src/Bookstore.IdentityServer/Bookstore.IdentityServer.csproj
dotnet run --project src/Bookstore.Api/Bookstore.Api.csproj
```

## Running tests

```bash
dotnet test tests/Bookstore.Api.Tests/Bookstore.Api.Tests.csproj
```

## Visual Studio

Add **Container Orchestrator Support → Docker Compose** to the solution
(right-click `Bookstore.sln` → Add) to make the compose stack the F5 target —
this is a one-time, IDE-driven step and isn't scripted here.

## Out of scope

Container registry publishing (images are built directly on the deploy
target, not pushed to a registry).
