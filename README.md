# Cloud‑Enabled Manufacturing Monitoring System
*(Simulated Telemetry & Real‑Time Metrics)*

## Overview

This project is a **cloud‑ready backend system** that simulates industrial machine telemetry and computes **real‑time and historical uptime/downtime metrics** on demand.

It is designed using **enterprise backend patterns** commonly found in manufacturing, oil & gas, and industrial IoT systems.

The system does **not integrate with real hardware**, but it is structured as if it could, making it suitable for demonstrating **professional backend engineering skills**.

---

## Key Features

- ✅ Simulated machine telemetry (Running / Stopped)
- ✅ Event‑driven, time‑series data modeling
- ✅ Dynamic uptime & downtime calculation (no pre‑aggregation)
- ✅ Rolling time windows (e.g., last 30 / 60 minutes)
- ✅ Current downtime streak tracking
- ✅ Clean layered architecture
- ✅ REST API with Swagger
- ✅ Unit‑tested business logic

---

## Architecture Overview

The system follows a clean, layered architecture that separates concerns and supports future scalability.

```mermaid
flowchart LR
    SIM["Telemetry Simulator<br/>(Background Service)"]
    API["API Layer<br/>ASP.NET Core Web API"]
    APP["Application Layer<br/>Metrics & History Services"]
    DOMAIN["Domain Layer<br/>Entities & Enums"]
    REPO["Telemetry Repository<br/>(In‑Memory / Pluggable)"]

    SIM --> REPO
    API --> APP
    APP --> REPO
    APP --> DOMAIN
    REPO --> DOMAIN
