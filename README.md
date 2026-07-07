# JCD ERP — SaaS Multi-Tenant

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-21-DD0031?logo=angular&logoColor=white)](https://angular.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-Private-red)]()

**JCD ERP** is a commercial **multi-tenant ERP SaaS platform** built for SMBs and mid-market companies in Latin America. It centralizes sales, inventory, purchasing, finance, and administration in a single modern system with enterprise-grade architecture.

> **Status:** Phase 1 in progress (~65%) — Auth, multi-tenancy, users, roles, and tenant settings are functional.  
> **Architecture:** Modular Monolith · Clean Architecture · DDD · CQRS · NgRx

---

## Highlights

| Capability | Description |
|------------|-------------|
| **Multi-tenant SaaS** | Shared database with strict `TenantId` isolation and global query filters |
| **Enterprise backend** | ASP.NET Core 9, CQRS with MediatR, FluentValidation, Result Pattern |
| **Modern frontend** | Angular 21 standalone components, NgRx Store + Effects + Facades |
| **Secure auth** | JWT + refresh tokens, BCrypt password hashing, role-based permissions |
| **Production-ready foundations** | Serilog, Swagger, health checks, rate limiting, Docker Compose |

---

## Tech stack

### Backend

| Technology | Purpose |
|------------|---------|
| ASP.NET Core 9 / .NET 9 | REST API |
| Entity Framework Core 9 | ORM & migrations |
| PostgreSQL 16 | Transactional database |
| Redis 7 | Cache / sessions (prepared) |
| MediatR | CQRS commands & queries |
| FluentValidation | Input validation |
| AutoMapper | DTO mapping |
| Serilog | Structured logging |
| JWT + BCrypt | Authentication |
| xUnit | Unit tests |

### Frontend

| Technology | Purpose |
|------------|---------|
| Angular 21 | SPA (standalone components) |
| NgRx 21 | Global state (auth, users, roles, settings) |
| RxJS | Async flows in effects |
| Reactive Forms | Form handling |
| SCSS + CSS variables | Dark / light theme |

### DevOps

| Technology | Purpose |
|------------|---------|
| Docker Compose | Local PostgreSQL + Redis |
| GitHub Actions | CI/CD (planned) |

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     FRONTEND (Angular 21 + NgRx)             │
│         Features · Layouts · Guards · Interceptors           │
└─────────────────────────────┬───────────────────────────────┘
                              │ HTTP REST
┌─────────────────────────────▼───────────────────────────────┐
│                      Jcd.Erp.Api                             │
│           Controllers · Middleware · Swagger                 │
└─────────────────────────────┬───────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────┐
│                   Jcd.Erp.Application                        │
│        Commands · Queries · Handlers · Validators            │
└─────────────────────────────┬───────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────┐
│                     Jcd.Erp.Domain                           │
│     Entities · Value Objects · Result Pattern · Rules        │
└─────────────────────────────┬───────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────┐
│          Jcd.Erp.Persistence · Jcd.Erp.Infrastructure        │
│         EF Core · Repositories · JWT · BCrypt · Redis          │
└─────────────────────────────────────────────────────────────┘
```

**Principles:** SOLID · DRY · KISS · YAGNI · DDD · CQRS · Repository + Specification · Domain Events · Result Pattern

---

## Repository structure

```
JCD-ERP-SaaS-multi-tenant/
├── src/
│   ├── backend/
│   │   ├── Jcd.Erp.Domain/
│   │   ├── Jcd.Erp.Application/
│   │   ├── Jcd.Erp.Persistence/
│   │   ├── Jcd.Erp.Infrastructure/
│   │   ├── Jcd.Erp.Api/
│   │   └── Jcd.Erp.Shared/
│   └── frontend/
│       └── jcd-erp-web/
├── tests/
│   ├── Jcd.Erp.Domain.Tests/
│   └── Jcd.Erp.Integration.Tests/
├── docker/
│   └── docker-compose.yml
└── README.md
```

---

## Getting started

### Prerequisites

| Tool | Version |
|------|---------|
| .NET SDK | 9.0+ |
| Node.js | 22.12+ |
| Angular CLI | 21 |
| Docker Desktop | Latest |

### 1. Start infrastructure

```bash
cd docker
docker compose up -d
```

> **Note:** PostgreSQL runs on port **5433** (mapped from container `5432`) to avoid conflicts with other local databases.

### 2. Start the API

```bash
cd src/backend
dotnet restore
dotnet run --project Jcd.Erp.Api
```

| Service | URL |
|---------|-----|
| API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| Health | http://localhost:5000/health |

Migrations and permission seeds run automatically on startup.

### 3. Start the frontend

```bash
cd src/frontend/jcd-erp-web
npm install
npm start
```

| App | URL |
|-----|-----|
| Web UI | http://localhost:4200 |
| Register | http://localhost:4200/auth/register |
| Login | http://localhost:4200/auth/login |

### 4. Test registration (API)

```bash
curl -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d "{\"companyName\":\"Acme Corp\",\"slug\":\"acme-corp\",\"adminEmail\":\"admin@acme.com\",\"adminPassword\":\"Password123!\",\"adminFirstName\":\"Admin\",\"adminLastName\":\"Acme\"}"
```

---

## API endpoints (Phase 1)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/v1/auth/register` | No | Register company + admin user |
| `POST` | `/api/v1/auth/login` | No | Sign in |
| `POST` | `/api/v1/auth/refresh` | No | Refresh access token |
| `POST` | `/api/v1/auth/logout` | Yes | Sign out |
| `POST` | `/api/v1/auth/change-password` | Yes | Change password |
| `GET` | `/api/v1/users` | Yes | List tenant users |
| `GET` | `/api/v1/roles` | Yes | List tenant roles |
| `GET` | `/api/v1/settings` | Yes | Tenant settings |
| `PUT` | `/api/v1/settings/{key}` | Yes | Update setting |

---

## Roadmap

| Phase | Scope | Status |
|-------|-------|--------|
| **0** | Architecture & planning | Done |
| **1** | Auth, multi-tenant, users, roles, settings | In progress |
| **2** | Master data (products, customers, suppliers) | Planned |
| **3** | Inventory & warehouses | Planned |
| **4** | Purchasing | Planned |
| **5** | Sales & invoicing | Planned |
| **6** | Finance (cash, banks) | Planned |
| **7** | Dashboard, reports, CI/CD | Planned |

---

## Security & multi-tenancy

- Every business table includes `tenant_id`
- EF Core global query filters enforce tenant isolation
- JWT access tokens (15 min) + refresh tokens (7 days)
- Granular permissions: `users.view`, `roles.create`, `settings.update`, etc.
- Rate limiting, CORS, structured logging, health checks

---

## Development conventions

| Layer | Namespace / pattern |
|-------|---------------------|
| Backend | `Jcd.Erp.*` |
| Frontend state | NgRx: Actions → Effects → Services → Reducer → Facade |
| API versioning | `/api/v1/` |
| Database | PostgreSQL, snake_case columns |

---

## License

Private commercial project. All rights reserved.

---

## Author

**JCD** — [GitHub @jcd321](https://github.com/jcd321)

Built as a production-grade SaaS portfolio and commercial product.
