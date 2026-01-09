# Deloitte.UrlShortener

A small URL shortener implemented with a **Clean Architecture** layout:

- **Domain** – core entities.
- **Application** – application logic, interfaces, and models.
- **Infrastructure** – storage implementations and wiring.
- **Api** – minimal HTTP API exposing redirect endpoints.
- **Api.Tests** – end‑to‑end tests for the API.

---

## Projects

### Deloitte.UrlShortener.Api

**Type:** ASP.NET Core Minimal API (`net10.0`)  
**What it does:**

- Hosts the HTTP service.
- Endpoints:
  - `GET /health` – returns `200 OK` with `{ status = "Healthy" }`.
  - `GET /{code}` – resolves a short code and returns an HTTP redirect:
    - existing code → 3xx redirect to the destination URL;
    - unknown code → `404 Not Found`;
    - invalid destination → `400 Bad Request`.

**Configuration:**

- `appsettings.json`:

  ```json
  "LinkStore": {
    "FilePath": "links.txt"
  },
  "LinkStore:Sqlite": {
    "FilePath": "links.db"
  }
