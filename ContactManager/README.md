# ContactManager

Two iterations live in this repo:

- **ContactManagerV1**: initial scratch app (single project)
- **ContactManagerV2**: service-architecture version (REST + JWT + gRPC + EF Core + tests + a few practical patterns)

> **Prereqs**
> - .NET SDK (net8.0)
> - EF Core CLI tool (`dotnet-ef`) available
>
> If you don’t have it:
> ```bash
> dotnet tool install --global dotnet-ef
> ```

---

## ContactManagerV2 (service architecture version)

### What it is

A basic CRUD contacts system with:

- **ContactManager.Api**: public REST API + JWT auth
- **ContactManager.Grpc**: internal gRPC service
- **ContactManager.Data**: EF Core model + migrations
- **SQLite**: persistence
- **Unit + integration tests**: automated verification
- **Design patterns**: Strategy / Decorator / Factory Method / Command (lightweight, practical)

### Architecture overview
```
Client (curl/Swagger)
       |
       v
ContactManager.Api (REST + JWT)
       |
       v
ContactManager.Grpc (gRPC)
       |
       v
SQLite (EF Core)
```


---

## Quickstart (V2)

### Build + test

```bash
dotnet clean
dotnet restore
dotnet build
dotnet test
```

## Database (V2): migrations + updates

### Create the initial migration (only once)

```bash
dotnet ef migrations add InitialCreate \
  --project src/ContactManager.Data/ContactManager.Data.csproj \
  --startup-project src/ContactManager.Api/ContactManager.Api.csproj
```

### Apply migrations for API startup

```bash
dotnet ef database update \
  --project src/ContactManager.Data/ContactManager.Data.csproj \
  --startup-project src/ContactManager.Api/ContactManager.Api.csproj
```

### Apply migrations for gRPC startup

```bash
dotnet ef database update \
  --project src/ContactManager.Data/ContactManager.Data.csproj \
  --startup-project src/ContactManager.Grpc/ContactManager.Grpc.csproj
```

## Run services (V2)

### Terminal 1 — Start gRPC service

```bash
dotnet run --project src/ContactManager.Grpc/ContactManager.Grpc.csproj
```
By default gRPC is typically on http://localhost:5055 unless you configured it differently.

### Terminal 2 — Start API service (points to gRPC)

```bash
GRPC_URL="http://localhost:5055"
dotnet run --project src/ContactManager.Api/ContactManager.Api.csproj --grpc_link "$GRPC_URL"
```
The API resolves gRPC address in this order:
1. CLI --grpc_link
2. config: Grpc:ContactsUrl
3. fallback: http://localhost:5055

## Manual API testing (V2) with curl

Set your API base URL (use the port your API prints on startup):
```bash
BASE="http://localhost:5000"
```

### 1) No token → should be 401
```bash
curl -i "$BASE/contacts"
```


### 2) Login → get JWT
```bash
TOKEN=$(curl -s -X POST "$BASE/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}' \
| python3 -c "import sys,json; print(json.load(sys.stdin)['accessToken'])")

echo "TOKEN=$TOKEN"
```

### 3) Create a contact (POST)
```bash
CREATE_RES=$(curl -s -i -X POST "$BASE/contacts" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Joshua","email":"josh@example.com","phone":"1234567890"}')

echo "$CREATE_RES"
```

### 4) Extract the created ID
```bash
ID=$(echo "$CREATE_RES" | tail -n 1 | python3 -c "import sys,json; print(json.load(sys.stdin)['id'])")
echo "ID=$ID"
```

### 5) List all contacts (GET)
```bash
curl -s -i "$BASE/contacts" -H "Authorization: Bearer $TOKEN"
```

### 6) Get one contact (GET)
```bash
curl -s -i "$BASE/contacts/$ID" -H "Authorization: Bearer $TOKEN"
```

### 7) Update (PUT)
```bash
curl -s -i -X PUT "$BASE/contacts/$ID" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Joshua Wiseman","email":"josh@example.com","phone":"1234567890"}'
```

### 8) Delete (DELETE)
```bash
curl -s -i -X DELETE "$BASE/contacts/$ID" \
  -H "Authorization: Bearer $TOKEN"
```

## Project phases (V2)

### Phase 1 — REST CRUD + validation
- Endpoints: `GET /contacts`, `GET /contacts/{id}`, `POST`, `PUT`, `DELETE`
- Validation: name required, email format, phone length constraints
- Pattern: *Adapter* (DTO ↔ entity mapping)

Done when:
- CRUD works in Swagger
- invalid requests return 400 with helpful messages

### Phase 2 — EF Core + SQLite + migrations
- `ContactDbContext` in `ContactManager.Data`
- SQLite file DB
- migrations + database update
- Pattern: *Facade* (`ContactService` hides DbContext details)

Done when:
- delete DB file → apply migrations → app works again

### Phase 3 — Auth with JWT
- `/auth/login` issues JWT (hardcoded user OK)
- CRUD protected with `[Authorize]`
- optional roles/policies
- Pattern: *Proxy/Decorator-ish* enforcement (or policies)

Done when:
- no token → 401
- token → CRUD works

### Phase 4 — Tests
- unit tests: validation + service logic
- integration tests: `WebApplicationFactory`, SQLite in-memory/temp DB, auth + CRUD flow

Done when:
- `dotnet test` is green and covers core flows

### Phase 5 — Add gRPC
- `contacts.proto`
- unary calls like `GetContact`, `UpsertContact`, `ListContacts` (and delete if implemented)
- gRPC hosted as separate service
- REST calls gRPC internally
- Patterns: *Decorator* (interceptors), *Facade* (REST over gRPC)

Done when:
- REST calls gRPC successfully
- deadlines/timeouts fail fast as expected

### Phase 6 — Practical patterns
Patterns used where they improve clarity (not ceremony):
- *Strategy*: search/filter policies
- *Decorator*: caching wrapper around contacts client/service
- *Factory Method*: choose storage provider via config
- *Command*: worker/job processing (optional)

Done when:
- patterns reduce complexity / improve maintainability
- you can explain why each exists

## ContactManagerV1 (initial scratch app)

### Build + run
```bash
dotnet clean
dotnet restore
dotnet build
dotnet run --project contact_manager_proj/contact_manager_proj.csproj
```

### Tests
```bash
dotnet test
```

### Release / publish
```bash
dotnet clean
dotnet build -c Release
dotnet publish contact_manager_proj/contact_manager_proj.csproj -c Release -r linux-x64 --self-contained false
```

Run the published output:
```bash
dotnet contact_manager_proj/bin/Release/net8.0/linux-x64/publish/contact_manager_proj.dll
```

