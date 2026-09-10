# Bookstore Backend

.NET 8 backend for a bookstore: Book CRUD (OAuth2 client-credentials flow) and
paginated book search (OAuth2 implicit flow, restricted to the `ProCredit`
claimed group).

## Live deployment

The full stack runs on a VPS at `http://109.199.122.254`:
- Swagger UI: http://109.199.122.254/swagger (nginx reverse-proxies port 80 to the API container)
- IdentityServer discovery document: http://109.199.122.254:5001/.well-known/openid-configuration

Testing works the same as local dev — open Swagger, click **Authorize**, and
use the client-credentials flow for CRUD or the implicit flow for search (see
[Test credentials](#test-credentials) below for the exact values). One
difference: IdentityServer's session cookie persists correctly once you log
in, so switching from `alice` to `bob` to compare their search access
requires an incognito/private window or clearing cookies first — there's no
logout page in this project yet.

Any push to `main` auto-deploys here: a GitHub Actions pipeline runs the test
suite, builds and pushes both app images to GHCR, then SSHes into the VPS to
pull and restart the stack (`.github/workflows/deploy.yml`).

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

HTTPS/TLS for the live deployment (it's a bare IP address, not a domain, so
Let's Encrypt can't issue a certificate for it) and a proper logout page for
switching test users without clearing cookies.
