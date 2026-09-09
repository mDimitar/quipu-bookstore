# Bookstore Backend

.NET 8 backend for a bookstore: Book CRUD (OAuth2 client-credentials flow) and
paginated book search (OAuth2 implicit flow, restricted to the `ProCredit`
claimed group). See `docs/superpowers/specs/2026-09-09-bookstore-backend-design.md`
for the full design.

## Prerequisites

- .NET 8 SDK
- Docker Desktop

## Running the full stack

```bash
cp .env.example .env
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

## Running tests

```bash
dotnet test tests/Bookstore.Api.Tests/Bookstore.Api.Tests.csproj
```

## Visual Studio

Add **Container Orchestrator Support → Docker Compose** to the solution
(right-click `Bookstore.sln` → Add) to make the compose stack the F5 target —
this is a one-time, IDE-driven step and isn't scripted here.

## Out of scope (see spec §10)

GitHub Actions CI/CD, container registry publishing, and VPS deployment are
covered by a separate design once this backend is verified working locally.
