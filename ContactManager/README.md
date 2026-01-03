# ContactManager

## ContactManagerV1

### Commands to get app working

Build Project
```bash
dotnet clean
dotnet restore
dotnet build
```

Run Tests and Project
`Reminder: Testing folder needs to reference project folder`
```bash
dotnet test
dotnet run --project contact_manager_proj/contact_manager_proj.csproj
```

Release Project
```bash
dotnet clean
dotnet build -c Release
dotnet publish contact_manager_proj/contact_manager_proj.csproj -c Release -r linux-x64 --self-contained false
```

Run Released Version
```bash
dotnet contact_manager_proj/bin/Release/net8.0/linux-x64/publish/contact_manager_proj.dll
```

## ContactManagerV2

### Project Phases

#### Phase 1 — REST CRUD + validation (Section 1 foundation)

Goal: “I can create/update/delete/list contacts with proper validation.”

Build:

- Endpoints: `GET /contacts`, `GET /contacts/{id}`, `POST`, `PUT`, `DELETE`

- Validation rules (start simple):

  - name required

  - email valid format

  - phone length constraints

Design pattern that naturally fits here

- *Adapter* (lightweight mapping): map REST DTOs ↔ domain/entity cleanly (don’t leak EF entities over the wire).

Done when

- CRUD works in Swagger

- Invalid requests return 400 with useful messages

#### Phase 2 — EF Core + SQLite + migrations (Section 1 persistence)

Goal: “Data actually persists.”

Build:

- `ContactDbContext` in `ContactManager.Data`

- SQLite for speed (single file DB)

- migrations (`dotnet ef migrations add InitialCreate`)

- repository optional (don’t over-abstract too early)

Pattern (optional)

- *Facade*: a `ContactService` that hides DbContext details from controllers.

Done when

- You can delete the DB file, run migrations, and everything still works

#### Phase 3 — Auth with JWT (Section 1 auth)

Goal: “Endpoints require auth.”

Build:

- `/auth/login` endpoint issues JWT (hardcoded user is fine for now)

- Protect CRUD endpoints with `[Authorize]`

- Roles optional (admin vs user)

Pattern

- *Proxy* / *Decorator-ish* (practical): a wrapper service that enforces auth/permissions before calling the core service logic (or use policies directly—both are fine).

Done when

- Without a token: 401

- With token: CRUD works

#### Phase 4 — Tests (this ties everything together)

Goal: “I can prove it works automatically.”

Build:

- Unit tests: validation + service logic

- Integration tests:

  - spin up API in-memory (`WebApplicationFactory`)
  
  - use SQLite in-memory or temp DB
  
  - test auth + one CRUD flow end-to-end

Pattern

- Strategy can show up as different validation strategies, but don’t force it yet.

Done when

- `dotnet test` runs green and covers core flows

#### Phase 5 — Add gRPC (Section 2)

Goal: “Internal comms via gRPC, REST remains public.”

Build:

- Define `contacts.proto`

- Start with unary calls:

  - `GetContact`
  
  - `UpsertContact`

- Host `ContactManager.Grpc` as its own service (separate process)

- `ContactManager.Api` calls gRPC internally (or vice versa)

Features to learn

- deadlines/timeouts

- interceptors (logging + timing)

- retry policy (optional)

Pattern

- *Decorator*: interceptor is basically a decorator for RPC calls.

- *Facade*: REST API acts as facade over internal gRPC.

Done when

- REST endpoint calls gRPC service successfully

- You can set a deadline and see it fail fast if exceeded

### Commands

Build Project
```bash
dotnet clean
dotnet restore
dotnet build
```

Run Tests
`Reminder: All these project reference one another in different ways.`
```bash
dotnet test
```

#### API

Build base DB schema called InitialCreate
```bash
dotnet ef migrations add InitialCreate \
  --project src/ContactManager.Data/ContactManager.Data.csproj \
  --startup-project src/ContactManager.Api/ContactManager.Api.csproj
```
  
Build DB
```bash
dotnet ef database update \
  --project src/ContactManager.Data/ContactManager.Data.csproj \
  --startup-project src/ContactManager.Api/ContactManager.Api.csproj
```


Run Project
```bash
dotnet run --project src/ContactManager.Api/ContactManager.Api.csproj
```

Confirm No Token Case
```bash
BASE="http://localhost:5249"
curl -i "$BASE/contacts"
```

Login and get a token
```bash
TOKEN=$(curl -s -X POST "$BASE/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password"}' \
| python3 -c "import sys,json; print(json.load(sys.stdin)['accessToken'])")

echo "TOKEN=$TOKEN"
```

Create a contact (POST)
```bash
CREATE_RES=$(curl -s -i -X POST "$BASE/contacts" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Joshua","email":"josh@example.com","phone":"1234567890"}')

echo "$CREATE_RES"
```

Extract Example ID
```bash
ID=$(echo "$CREATE_RES" | tail -n 1 | python3 -c "import sys,json; print(json.load(sys.stdin)['id'])")
echo "ID=$ID"
```

List All Contacts (GET)
```bash
curl -s -i "$BASE/contacts" -H "Authorization: Bearer $TOKEN"
```

List Contact (GET)
```bash
curl -s -i "$BASE/contacts/$ID" -H "Authorization: Bearer $TOKEN"
```

Update (PUT)
```bash
curl -s -i -X PUT "$BASE/contacts/$ID" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Joshua Wiseman","email":"josh@example.com","phone":"1234567890"}'
```

Delete (DELETE)
```bash
curl -s -i -X DELETE "$BASE/contacts/$ID" -H "Authorization: Bearer $TOKEN"
```

#### GRPC

Build DB
```bash
dotnet ef database update \
  --project src/ContactManager.Data/ContactManager.Data.csproj \
  --startup-project src/ContactManager.Grpc/ContactManager.Grpc.csproj
```

Start GRPC Service
```bash
dotnet run --project src/ContactManager.Grpc/ContactManager.Grpc.csproj --urls http://localhost:5055
```

Then Access Api that uses GRPC
```bash
dotnet run --project src/ContactManager.Api/ContactManager.Api.csproj
```
