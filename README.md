# BatchLab – Async Batch Processing Lab

## Overview

BatchLab is a cloud‑native async batch processing system. Users submit batch jobs through an API and a simple UI. Jobs are queued, processed asynchronously, and their status can be tracked end‑to‑end.

The goal is **real delivery in 4 consecutive days**, with strict scope control, clear ownership, and managed services only (Codespaces‑friendly).

---

## Final Stack

### Backend

* .NET 8 – ASP.NET Core Minimal API
* AWS SDK for .NET
* Messaging: AWS SQS (Standard Queue)
* Persistence: AWS DynamoDB

### Frontend

* Angular (standalone app, no Nx)

### Environment

* GitHub Codespaces
* Run via `dotnet run` and `ng serve`
* Configuration via environment variables

### Explicitly Out of Scope

* Authentication / Authorization
* Redis / caching
* Observability stack
* Infrastructure as Code
* Docker as a dev requirement

---

## Final Artifact

### What the system does

1. User submits a batch job
2. API stores job as `Queued`
3. API publishes message to SQS
4. Worker consumes and processes the job
5. Job status is updated
6. UI displays job status

This flow **must work end‑to‑end** for the project to be considered delivered.

---

## MVP – Non‑Negotiable Scope

### Backend

* `POST /jobs` – create a batch job
* `GET /jobs/{id}` – retrieve job status
* Publish messages to SQS
* Background worker consuming SQS
* Persist job status in DynamoDB

### Frontend

* Simple form to create a job
* Status view for a single job

### MVP Success Criteria

* Full async flow works without manual intervention

---

## Architecture Layers

### Layer 1 – Core (Must Not Fail)

* API accepts job requests
* Message is published to SQS
* Worker consumes message
* Processing is logged

### Layer 2 – Demonstrable Value

* Job status persisted in DynamoDB
* Status retrieval endpoint
* Minimal UI connected to API

### Layer 3 – Optional Extras (Only if Time Remains)

* Better UX
* Retry logic
* Job listing

---

## Roles & Ownership

### Daniel – Backend & Async Core

* Overall architecture
* .NET API
* SQS integration
* Worker implementation
* DynamoDB persistence
* Owner of Layer 1 (core reliability)

### Gabriel – Frontend & Integration

* Angular UI
* UI ↔ API integration
* User flow and basic UX
* Backend support when needed

Rule: **each person owns their layer**. Cross‑layer changes require alignment.

---

## 4‑Day Execution Plan (With Parallel Work)

### Day 1 – Feasibility & Skeleton

**Daniel**

* Create repository structure
* Bootstrap .NET Minimal API
* Configure AWS SDK access
* Create SQS queue
* Publish and consume a test message (logs only)

**Gabriel**

* Bootstrap Angular project
* Create basic layout (form + status view)
* Define API contract with Daniel

**Success Criteria**

* API → SQS → Worker flow visible in logs

**Cut Line**

* If persistence blocks progress, keep job status in memory

---

### Day 2 – Functional Core

**Daniel**

* Create DynamoDB table
* Persist job status
* Implement `POST /jobs` and `GET /jobs/{id}`
* Connect worker processing to status updates

**Gabriel**

* Implement job creation UI
* Connect UI to `POST /jobs`
* Mock status responses if backend lags

**Success Criteria**

* Full async flow works via Postman or curl

**Cut Line**

* If UI falls behind, backend stability takes priority

---

### Day 3 – UI Integration & Stabilization

**Daniel**

* Error handling (basic)
* Idempotency safeguards (minimal)
* Worker stability checks

**Gabriel**

* Implement job status polling
* Display job state transitions
* Basic UX cleanup

**Success Criteria**

* Demo works end‑to‑end without explanation

**Cut Line**

* UI polish is optional; correctness is not

---

### Day 4 – Cleanup & Delivery

**Daniel**

* Code cleanup
* Configuration review
* Sanity testing

**Gabriel**

* Final UI cleanup
* Demo flow preparation

**Joint Tasks**

* Write README
* Document setup steps
* Prepare demo narrative

**Success Criteria**

* A third party can run the project in Codespaces and understand it

---

## Expected Outcome

* A working async batch processing MVP
* Clear async architecture using managed services
* Real, transferable backend experience
* A finished artifact, not a prototype sketch
