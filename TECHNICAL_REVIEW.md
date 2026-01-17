# Portfolio Project Evaluation: BatchLab

**Evaluator Context:** Senior Technical Recruiter with Backend Engineering Background  
**Company Type:** International FinTech  
**Project Type:** Personal Learning / Portfolio Project  
**Evaluation Date:** January 17, 2026

---

## Project Context

BatchLab is a cloud-native asynchronous batch processing system built as a **4-day learning project** to demonstrate understanding of distributed systems patterns. The candidate explicitly scoped this as a demonstration artifact, not production software.

**Stack:** .NET 10, AWS SQS, AWS DynamoDB, Angular  
**Architecture:** API → Message Queue → Background Worker → Persistence

---

## 1. Failure Mode Awareness

### Positive Signals

**The candidate clearly understands that distributed systems fail in complex ways:**

- **Idempotency awareness**: The Worker checks job status before processing (`if(status != "Pending")`) and uses DynamoDB conditional expressions (`ConditionExpression = "#S = 'Pending'"`) to prevent double-completion. This shows understanding that messages can be delivered more than once.

- **Granular exception handling**: Instead of a generic catch-all, the Worker distinguishes between:
  - `JsonException` (bad message format)
  - `ConditionalCheckFailedException` (race condition detected)
  - `ReceiptHandleIsInvalidException` (message already processed/expired)
  
  This signals understanding that different failures require different responses.

- **Message deletion after processing**: The Worker correctly deletes messages only after successful processing, not before. This is the correct SQS consumption pattern.

### Implicit Understanding (Inferred)

- The TODO comment `//TO-DO: Graceful shutdown` indicates awareness that abrupt termination causes issues in message processing systems
- The TODO `//TO-DO: Change status here to failed or completed based on the exception type and retry logic` shows the candidate knows retry policies should be failure-type-aware
- The TODO `//TO-DO: Add transactional idempotency for preventing server states changes on duplicate` reveals understanding of the dual-write problem

### Interview Follow-up Questions

1. "You have conditional expressions preventing double-completion. Walk me through what happens if two Workers pick up the same message before either finishes processing."
2. "Your TODOs mention retry logic—what retry strategy would you implement here, and why?"
3. "How would you handle a message that fails processing 5 times in a row?"

---

## 2. Architectural Coherence

### Positive Signals

**The architecture demonstrates intentional, coherent design choices:**

- **Clean separation of concerns**: 
  - `Domain/` for entities
  - `Dto/` for API contracts  
  - `Service/` for business logic
  - `Infrastructure/` for AWS integrations
  - Interfaces defined for all infrastructure (`IJobsRepository`, `IMessageBus`)

- **Dependency injection throughout**: Services are registered in DI container with appropriate lifetimes (`Singleton` for DynamoDB client, `Scoped` for services). This shows understanding of object lifecycle management.

- **Minimal API design**: Endpoints are concise and focused. The candidate chose ASP.NET Minimal APIs over controllers—a deliberate simplicity choice that matches the project scope.

- **Worker as separate process**: The Worker is a standalone project (`BatchLabWorker/`), not embedded in the API. This is the correct architecture for independent scaling and failure isolation.

### Implicit Understanding (Inferred)

- The interface-based design (`IMessageBus`, `IJobsRepository`) suggests the candidate understands dependency inversion and testability, even without tests present
- Having `JsonFileRepository` alongside `DynamoDBRepository` shows awareness that infrastructure should be swappable (useful for local development)
- The README's cost modeling section reveals the candidate thinks about operational concerns, not just code

### Interview Follow-up Questions

1. "You separated the Worker into its own project. What trade-offs did you consider versus running it as a BackgroundService in the API process?"
2. "Your interfaces are tightly coupled to `JobEntity`. How would you evolve this if you needed to handle different job types?"
3. "Walk me through how you'd add a second message consumer for a different queue."

---

## 3. Distributed Systems Primitives

### Positive Signals

**The candidate demonstrates working knowledge of async messaging patterns:**

- **Long polling configuration**: `WaitTimeSeconds = 20` is the AWS-recommended setting for SQS consumption. This isn't accidental—it's the documented best practice.

- **At-least-once delivery handling**: The status check + conditional update pattern is a valid approach to handling SQS's delivery guarantees. The candidate didn't assume exactly-once.

- **Optimistic concurrency control**: Using DynamoDB conditions instead of locks is the scalable pattern for distributed state management. This is the right instinct.

- **Minimal message payloads**: The full `JobEntity` is in the message, but the Worker fetches current status from the database before acting. This shows understanding that message content can be stale.

### Implicit Understanding (Inferred)

- The README states "at-least-once delivery model" explicitly—the candidate researched SQS semantics
- Choosing DynamoDB On-Demand mode suggests awareness of unpredictable workload patterns
- The cost calculation showing "with batching" vs "without batching" indicates understanding of throughput optimization

### Interview Follow-up Questions

1. "You check status from DynamoDB before processing. Why not trust the message content alone?"
2. "SQS Standard Queue can deliver messages out of order. Does your design handle that? Would it matter for this use case?"
3. "If you needed exactly-once processing for financial transactions, what would you change?"

---

## 4. Code Quality & Pragmatism

### Positive Signals

**The code reflects practical, maintainable choices:**

- **Consistent naming conventions**: PascalCase for classes/methods, camelCase for variables, `I` prefix for interfaces. The candidate follows .NET conventions.

- **Argument validation**: `ArgumentNullException.ThrowIfNull()` and `ArgumentException.ThrowIfNullOrEmpty()` are used consistently. Defensive programming is present.

- **Records for DTOs**: Using C# records (`JobDto`) for immutable data transfer shows awareness of modern C# features.

- **Global exception handler**: The API has centralized error handling that returns structured responses with appropriate status codes. This is production-pattern thinking.

- **Explicit scope documentation**: The README clearly states what's out of scope (auth, Redis, WebSockets, observability). This is senior-level communication—setting expectations rather than apologizing for gaps.

### Implicit Understanding (Inferred)

- The comment `//TO-DO: Create mapper between JobDto and JobEntity` shows awareness of the DTO/Entity separation pattern, even if AutoMapper isn't implemented
- Hardcoded values have TODO comments (`//TO-DO: Move AWS config to appsettings.json`)—the candidate knows this isn't ideal but prioritized working code
- The 4-day timeline constraint is documented, showing realistic scope management

### Interview Follow-up Questions

1. "You have similar `DynamoDBRepository` implementations in both projects. How would you share this code in a real system?"
2. "Your exception handler maps exceptions to status codes. How would you handle validation errors differently from infrastructure failures?"
3. "What would you add first if you had two more days?"

---

## 5. Awareness Signals (TODO Analysis)

The TODO comments throughout the codebase are **positive signals**, not defects. They demonstrate:

| TODO | What It Signals |
|------|-----------------|
| `//TO-DO: Graceful shutdown` | Understands process lifecycle management |
| `//TO-DO: Add transactional idempotency` | Aware of dual-write consistency problems |
| `//TO-DO: Move AWS config to appsettings.json` | Knows configuration should be externalized |
| `//TO-DO: Dispose AmazonSQSClient properly` | Understands resource management |
| `// TODO: Add thread-safety mechanisms` | Recognizes concurrency risks in file I/O |
| `//TO-DO: Generic message bus interface` | Sees abstraction improvement opportunities |

**This TODO discipline is uncommon.** Many candidates either leave no comments or don't recognize what's missing. This candidate annotated technical debt as they incurred it.

---

## 6. Documentation & Communication

### Positive Signals

- **Comprehensive README**: Architecture overview, conventions, cost modeling, execution plan, scalability targets
- **Explicit trade-offs documented**: "real delivery over ideal architecture"
- **Role ownership defined**: Clear boundaries between team members
- **Recruiter-facing section**: The candidate anticipates who will read this and why

This level of documentation on a 4-day project is **exceptional**. It signals someone who communicates proactively and thinks about project context, not just code.

---

## Final Assessment

### Overall Seniority Signal: **Strong Mid-Level**

The project demonstrates:
- ✅ Solid understanding of async messaging patterns
- ✅ Correct instincts for distributed state management (optimistic concurrency)
- ✅ Clean architectural boundaries and DI usage
- ✅ Awareness of failure modes (documented via TODOs and exception handling)
- ✅ Pragmatic scope management with explicit trade-offs
- ✅ Strong technical communication skills

The candidate is **beyond junior** (shows systems thinking, not just CRUD) but the implementations are still learning-stage. A senior would likely have implemented at least basic retry logic even in a demo project.

---

### Does This Strengthen the Candidate's Profile for FinTech Backend?

**Yes, meaningfully.**

This project demonstrates the candidate:
1. Can reason about eventual consistency and idempotency
2. Understands cloud-managed infrastructure (SQS, DynamoDB)
3. Communicates technical decisions clearly
4. Knows what production systems need (even if not implemented here)

For a **mid-level backend role** in FinTech, this project would move them to the interview stage. For a **senior role**, it provides good talking points but would need to be supplemented by interview depth.

---

### Top 2 Improvements to Increase Signal (Not Completeness)

#### 1. Add one integration test proving the async loop works

A single test that:
- Creates a job via API
- Waits for Worker to process it
- Verifies status changed to "Completed"

This would demonstrate the candidate can validate distributed flows, not just write them. It's ~30 lines of code but dramatically increases confidence.

#### 2. Implement basic retry with backoff in the Worker

Even a simple 3-retry loop with `Task.Delay(attempt * 1000)` would show the candidate can translate their documented awareness (the TODOs) into working code. This is the gap between "knows what to do" and "does it."

---

## Summary for Hiring Committee

| Dimension | Signal |
|-----------|--------|
| Distributed Systems Fundamentals | ✅ Strong |
| Code Organization | ✅ Strong |
| Cloud Infrastructure Knowledge | ✅ Adequate |
| Failure Mode Awareness | ✅ Present (documented, partially implemented) |
| Production Readiness | ⚪ Not applicable (portfolio project) |
| Communication | ✅ Exceptional for project scope |

**Recommendation:** Proceed to technical interview. Use the project as a discussion anchor for distributed systems depth. The candidate shows potential to grow into a senior backend role with the right mentorship and production exposure.

---

*This evaluation assesses the project as a learning artifact, not production software. The candidate explicitly scoped this as a 4-day demonstration project.*
