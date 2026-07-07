# JCD ERP — SaaS Multi-Tenant

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-21-DD0031?logo=angular&logoColor=white)](https://angular.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-Private-red)]()

**JCD ERP** is a commercial **multi-tenant ERP SaaS platform** built for SMBs and mid-market companies in Latin America. It centralizes sales, inventory, purchasing, finance, and administration in a single modern system with enterprise-grade architecture.

> **Status:** Phase 2 in progress — Master data catalogs. Phase 1 complete (auth, users, roles, settings, Redis, i18n, CI).  
> **Architecture:** Modular Monolith · Clean Architecture · DDD · CQRS · NgRx

---

## Highlights

| Capability | Description |
|------------|-------------|
| **Multi-tenant SaaS** | Shared database with strict `TenantId` isolation and global query filters |
| **Enterprise backend** | ASP.NET Core 9, CQRS with MediatR, FluentValidation, Result Pattern |
| **Modern frontend** | Angular 21 standalone components, NgRx Store + Effects + Facades |
| **Secure auth** | JWT + refresh tokens, BCrypt, forgot/reset password, role-based permissions |
| **Tenant isolation** | EF Core global query filters per request + tenant resolution middleware |
| **Production-ready foundations** | Serilog, Swagger, health checks, rate limiting, Docker Compose, Redis cache |

---

## Tech stack

### Backend

| Technology | Purpose |
|------------|---------|
| ASP.NET Core 9 / .NET 9 | REST API |
| Entity Framework Core 9 | ORM & migrations |
| PostgreSQL 16 | Transactional database |
| Redis 7 | Cache (permissions, tenant settings) |
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
| GitHub Actions | CI/CD (build + test on push/PR) |

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
| Forgot password | http://localhost:4200/auth/forgot-password |

> **Tip:** If `dotnet run` fails with *"file is locked by Jcd.Erp.Api"*, stop the running instance first:
> `Get-Process -Name "Jcd.Erp.Api" -ErrorAction SilentlyContinue | Stop-Process -Force`

### 4. Run tests

Requires Docker (PostgreSQL on port **5433** and Redis on **6379**):

```bash
# Create isolated test database (first time only)
psql -h localhost -p 5433 -U postgres -c "CREATE DATABASE jcd_erp_test;"

cd src/backend
dotnet test Jcd.Erp.sln
```

Integration tests use `appsettings.Testing.json` and database `jcd_erp_test` so your dev data stays untouched.

### 5. Test registration (API)

```bash
curl -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d "{\"companyName\":\"Acme Corp\",\"slug\":\"acme-corp\",\"adminEmail\":\"admin@acme.com\",\"adminPassword\":\"Password123!\",\"adminFirstName\":\"Admin\",\"adminLastName\":\"Acme\"}"
```

---

## API endpoints (Phase 1)

### Auth

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/v1/auth/register` | No | Register company + admin user |
| `POST` | `/api/v1/auth/login` | No | Sign in |
| `POST` | `/api/v1/auth/refresh` | No | Refresh access token |
| `POST` | `/api/v1/auth/logout` | Yes | Sign out |
| `POST` | `/api/v1/auth/change-password` | Yes | Change password (authenticated) |
| `POST` | `/api/v1/auth/forgot-password` | No | Request password reset link |
| `POST` | `/api/v1/auth/reset-password` | No | Set new password with reset token |

### Platform

| Method | Endpoint | Auth | Permission | Description |
|--------|----------|------|------------|-------------|
| `GET` | `/api/v1/users` | Yes | `users.view` | List tenant users |
| `POST` | `/api/v1/users` | Yes | `users.create` | Create user |
| `PUT` | `/api/v1/users/{id}` | Yes | `users.update` | Update user |
| `DELETE` | `/api/v1/users/{id}` | Yes | `users.delete` | Soft-delete user |
| `GET` | `/api/v1/roles` | Yes | `roles.view` | List tenant roles |
| `GET` | `/api/v1/roles/permissions` | Yes | `roles.view` | List available permissions |
| `POST` | `/api/v1/roles` | Yes | `roles.create` | Create role |
| `PUT` | `/api/v1/roles/{id}` | Yes | `roles.update` | Update role |
| `DELETE` | `/api/v1/roles/{id}` | Yes | `roles.delete` | Delete role |
| `GET` | `/api/v1/settings` | Yes | `settings.view` | Tenant settings |
| `PUT` | `/api/v1/settings/{key}` | Yes | `settings.update` | Update setting |

### Catalogs (Phase 2)

| Method | Endpoint | Auth | Permission | Description |
|--------|----------|------|------------|-------------|
| `GET` | `/api/v1/units` | Yes | `units.view` | List units (paginated, search) |
| `POST` | `/api/v1/units` | Yes | `units.create` | Create unit of measure |
| `PUT` | `/api/v1/units/{id}` | Yes | `units.update` | Update unit |
| `DELETE` | `/api/v1/units/{id}` | Yes | `units.delete` | Soft-delete unit |
| `GET` | `/api/v1/categories` | Yes | `categories.view` | List categories (paginated, search) |
| `GET` | `/api/v1/categories/parent-options` | Yes | `categories.view` | Parent category options for forms |
| `POST` | `/api/v1/categories` | Yes | `categories.create` | Create category |
| `PUT` | `/api/v1/categories/{id}` | Yes | `categories.update` | Update category |
| `DELETE` | `/api/v1/categories/{id}` | Yes | `categories.delete` | Soft-delete category |
| `GET` | `/api/v1/brands` | Yes | `brands.view` | List brands (paginated, search) |
| `POST` | `/api/v1/brands` | Yes | `brands.create` | Create brand |
| `PUT` | `/api/v1/brands/{id}` | Yes | `brands.update` | Update brand |
| `DELETE` | `/api/v1/brands/{id}` | Yes | `brands.delete` | Soft-delete brand |

---

## Roadmap

| Phase | Scope | Status |
|-------|-------|--------|
| **0** | Architecture & planning | Done |
| **1** | Auth, multi-tenant, users, roles, settings | **Done** |
| **2** | Master data (products, customers, suppliers) | **In progress** — Units, categories & brands done |
| **3** | Inventory & warehouses | Planned |
| **4** | Purchasing | Planned |
| **5** | Sales & invoicing | Planned |
| **6** | Finance (cash, banks) | Planned |
| **7** | Dashboard, reports, CI/CD | Planned |

### Phase 1 — delivered so far

| Area | Done |
|------|------|
| Auth | Register, login, refresh, logout, change/forgot/reset password |
| Multi-tenant | Global query filters, tenant middleware, JWT `tenant_id` claim |
| Users | Full CRUD with roles (modal UI + NgRx) |
| Roles | Full CRUD with permission assignment |
| Settings | List + update |
| Frontend | Design system, dark/light theme, permission guard, sidebar by role |
| DevOps | Docker Compose (PostgreSQL + Redis), GitHub Actions CI |
| Cache | Redis — user permissions (30 min TTL), tenant settings (1 h TTL) |

### Phase 1 — remaining (non-blocking)

| Item | Status |
|------|--------|
| SMTP email for production | Pending |
| API container in Docker Compose | Pending |

---

## Security & multi-tenancy

- Every business table includes `tenant_id`
- EF Core global query filters bind `CurrentTenantId` per request (fail-closed when tenant is missing)
- `TenantResolutionMiddleware` resolves tenant from JWT or authenticated user
- JWT access tokens (15 min) + refresh tokens (7–30 days)
- Password reset tokens (60 min, hashed in DB)
- Granular permissions: `users.view`, `roles.create`, `settings.update`, etc.
- Frontend `permissionGuard` protects routes; sidebar hides unauthorized modules
- Rate limiting, CORS, structured logging, health checks
- Redis cache for user permissions (30 min) and tenant settings (1 h), with invalidation on CRUD

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
