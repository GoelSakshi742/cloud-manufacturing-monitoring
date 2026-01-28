# 🏭 Cloud Manufacturing Monitoring System

A clean‑architecture based **ASP.NET Core Web API** that simulates manufacturing machine telemetry, stores events in memory, computes machine uptime/downtime metrics, and exposes insights via REST APIs.

---

## 📌 Key Features

- ✅ Real‑time telemetry simulation
- ✅ Machine uptime & downtime calculation
- ✅ Status timeline generation
- ✅ Clean Architecture (API, Application, Domain, Infrastructure)
- ✅ In‑memory repository for fast testing
- ✅ Fully unit‑tested business logic
- ✅ Swagger/OpenAPI support

---

## 🧱 Solution Architecture

The solution follows **Clean Architecture**, ensuring separation of concerns, testability, and scalability.

### 🔷 Layer Responsibilities

| Layer | Responsibility |
|-----|---------------|
| **API** | HTTP endpoints, DTO mapping, request validation |
| **Application** | Business logic, use cases, interfaces |
| **Domain** | Core entities, enums, business rules |
| **Infrastructure** | Data storage, simulations, external services |
| **Tests** | Unit tests with fake repositories |

---

## 🧩 Architecture Diagram (Mermaid)

```mermaid
flowchart TB

Client["Client / Browser / API Consumer"]

Client --> API["API Layer<br/>(MachinesController)"]

API --> App["Application Layer<br/>(Services & Interfaces)"]

App --> Domain["Domain Layer<br/>(Entities & Enums)"]

App --> Infra["Infrastructure Layer<br/>(Repositories & Simulation)"]

Infra --> Domain

Infra --> Store["In‑Memory Telemetry Store"]

Infra --> Simulator["TelemetrySimulationService<br/>(Background Worker)"]
