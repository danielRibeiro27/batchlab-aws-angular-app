# BatchLab – Async Batch Processing Lab

## 1. Overview

BatchLab is a **cloud‑native asynchronous batch processing system** designed to be built and delivered in **4 consecutive days** by two developers.

The project prioritizes:
- real delivery over ideal architecture
- managed cloud services
- explicit trade‑offs
- end‑to‑end functionality

The final artifact is intentionally simple, but architecturally honest, aiming to demonstrate **production‑grade async thinking** to recruiters.

---

## 2. Final Stack (Codespaces‑Friendly)

### Backend
- .NET 8
- ASP.NET Core Minimal API
- AWS SDK for .NET
- Messaging: **AWS SQS (Standard Queue)**
- Persistence: **AWS DynamoDB (On‑Demand)**

### Frontend
- Angular (standalone app)
- Simple form + status view

### Environment
- GitHub Codespaces only
- No local installations required
- Run with:
  - `dotnet run`
  - `ng serve`
- Configuration via environment variables

---

## 3. Coding Conventions

This section outlines the coding standards and best practices for both backend (.NET) and frontend (Angular) development.

### 3.1 Backend (.NET) Conventions

#### Naming Conventions
- **Classes, Interfaces, Methods (public), Properties**: `PascalCase`
  - Examples: `JobService`, `IJobRepository`, `CreateJobAsync`, `JobId`
- **Interfaces**: Prefix with `I`
  - Example: `IJobService`, `IMessagePublisher`
- **Variables (local), Parameters**: `camelCase`
  - Examples: `jobRequest`, `cancellationToken`, `jobId`

#### Folder Structure
```
Backend/
├── Api/                    # Endpoints and controllers
├── Services/               # Business logic
├── Models/                 # DTOs and entities
├── Infrastructure/         # AWS integrations (SQS, DynamoDB)
│   ├── Messaging/         # SQS publishers and consumers
│   └── Persistence/       # DynamoDB repositories
└── Configuration/         # Settings and DI setup
```

#### Code Principles
- **Minimal APIs**: Use minimal API syntax for endpoint definitions
  ```csharp
  app.MapPost("/jobs", async (CreateJobRequest request, IJobService service) => 
      await service.CreateJobAsync(request));
  
  app.MapGet("/jobs/{id}", async (Guid id, IJobService service) =>
      await service.GetJobStatusAsync(id));
  ```

- **Dependency Injection**: All services must be registered in DI container
  ```csharp
  builder.Services.AddSingleton<IJobService, JobService>();
  builder.Services.AddScoped<IJobRepository, DynamoDbJobRepository>();
  ```

- **Async/Await**: Always use async operations for I/O-bound work
  ```csharp
  public async Task<JobStatusResponse> GetJobStatusAsync(Guid jobId, CancellationToken ct)
  {
      var job = await _repository.GetByIdAsync(jobId, ct);
      return new JobStatusResponse(job.Id, job.Status, job.CreatedAt);
  }
  ```

- **Records for DTOs**: Use records for immutable data transfer objects
  ```csharp
  public record CreateJobRequest(string Name, string Payload);
  public record JobStatusResponse(Guid Id, string Status, DateTime CreatedAt);
  public record JobMessage(Guid JobId);
  ```

#### Error Handling
- **Result Pattern**: Return results instead of throwing exceptions for expected errors
- **ProblemDetails**: Use standard ProblemDetails for API error responses
  ```csharp
  if (!validationResult.IsValid)
      return Results.ValidationProblem(validationResult.Errors);
  ```

#### SQS Messaging
- **Minimal Messages**: Keep messages small with only essential IDs
  ```csharp
  var message = new JobMessage(jobId);
  await _sqsPublisher.PublishAsync(message, cancellationToken);
  ```
- **JSON Serialization**: Use `System.Text.Json` for message serialization
- **Long Polling**: Workers should use long polling (WaitTimeSeconds = 20)

#### DynamoDB
- **Naming**: Use PascalCase for table names and attributes
  - Table: `Jobs`, Attributes: `JobId`, `Status`, `CreatedAt`
- **PK/SK Pattern**: Use partition key (PK) and sort key (SK) pattern when needed
- **Attributes**: Keep attribute names consistent with C# property names
  ```csharp
  var item = new Dictionary<string, AttributeValue>
  {
      ["JobId"] = new AttributeValue { S = job.Id.ToString() },
      ["Status"] = new AttributeValue { S = job.Status },
      ["CreatedAt"] = new AttributeValue { S = job.CreatedAt.ToString("o") }
  };
  ```

---

### 3.2 Frontend (Angular) Conventions

#### Naming Conventions
- **Files and Selectors**: `kebab-case`
  - Examples: `job-form.component.ts`, `job-status.component.ts`
  - Selector: `app-job-form`, `app-job-status`
- **Classes and Interfaces**: `PascalCase`
  - Examples: `JobFormComponent`, `JobService`, `Job`, `JobStatusResponse`
- **Variables, Methods, Properties**: `camelCase`
  - Examples: `jobId`, `submitJob()`, `isLoading`

#### Folder Structure
```
Frontend/src/app/
├── core/                   # Singletons (services, guards, interceptors)
│   ├── services/
│   ├── models/
│   └── interceptors/
├── features/              # Feature modules
│   ├── job-submission/
│   └── job-status/
├── shared/                # Shared components
└── app.component.ts
```

#### Code Principles
- **Standalone Components**: Use standalone components (Angular 14+)
  ```typescript
  @Component({
    selector: 'app-job-form',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './job-form.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
  })
  export class JobFormComponent {
    // Component logic
  }
  ```

- **Reactive Forms**: Use Reactive Forms for form handling
  ```typescript
  jobForm = new FormGroup({
    name: new FormControl('', [Validators.required]),
    payload: new FormControl('', [Validators.required])
  });
  ```

- **Observables with RxJS**: Use Observables for async operations
  ```typescript
  submitJob(): void {
    this.jobService.createJob(this.jobForm.value)
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => {
        this.router.navigate(['/status', response.id]);
      });
  }
  ```

- **OnPush Change Detection**: Use OnPush strategy for better performance
  ```typescript
  changeDetection: ChangeDetectionStrategy.OnPush
  ```

#### Services
- **ProvidedIn Root**: Services should be provided in root for singleton behavior
  ```typescript
  @Injectable({
    providedIn: 'root'
  })
  export class JobService {
    // Service logic
  }
  ```

- **Single Responsibility**: Each service should have a single, well-defined purpose
- **Return Observables**: Service methods should return Observables
  ```typescript
  getJobStatus(id: string): Observable<JobStatusResponse> {
    return this.http.get<JobStatusResponse>(`/api/jobs/${id}`);
  }
  ```

#### Components
- **One Component Per File**: Each component in its own file
- **Maximum 400 Lines**: Keep components under 400 lines; refactor if larger
- **Inline Templates**: Only use inline templates if less than 10 lines
  ```typescript
  // Inline template (only if < 10 lines)
  template: `<div>{{ job.status }}</div>`
  
  // Separate file (preferred for > 10 lines)
  templateUrl: './job-status.component.html'
  ```

#### Polling Strategy
- **interval() with switchMap()**: Use RxJS interval for polling
  ```typescript
  private pollJobStatus(id: string): void {
    interval(5000).pipe(
      switchMap(() => this.jobService.getJobStatus(id)),
      takeUntil(this.destroy$)
    ).subscribe(status => {
      this.jobStatus.set(status);
      if (status.status === 'COMPLETED' || status.status === 'FAILED') {
        this.destroy$.next();
      }
    });
  }
  ```

- **Always use takeUntil()**: Prevent memory leaks by completing subscriptions
  ```typescript
  private destroy$ = new Subject<void>();
  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  ```

#### Typing
- **Always Type Returns**: Type all method return values
  ```typescript
  getJob(id: string): Observable<Job> { }
  calculateTotal(items: Item[]): number { }
  ```
- **Type Public Properties**: Always type public properties
  ```typescript
  jobStatus: JobStatusResponse | null = null;
  isLoading: boolean = false;
  ```
- **Avoid `any`**: Never use `any` type; use `unknown` if type is truly unknown
- **Use Interfaces**: Define interfaces for all data structures
  ```typescript
  export interface Job {
    id: string;
    name: string;
    status: 'QUEUED' | 'PROCESSING' | 'COMPLETED' | 'FAILED';
    createdAt: Date;
  }
  ```

#### Templates
- **Use Angular 17+ Syntax**: Prefer new control flow syntax
  ```html
  <!-- Conditionals -->
  @if (job) {
    <div>{{ job.status }}</div>
  } @else {
    <div>Loading...</div>
  }
  
  <!-- Loops -->
  @for (job of jobs; track job.id) {
    <app-job-item [job]="job" />
  }
  
  <!-- Switch -->
  @switch (job.status) {
    @case ('QUEUED') { <span>Queued</span> }
    @case ('PROCESSING') { <span>Processing</span> }
    @case ('COMPLETED') { <span>Completed</span> }
    @default { <span>Unknown</span> }
  }
  ```

---

### 3.3 General Conventions

#### Versioning
- **Conventional Commits**: Follow Conventional Commits specification
  - `feat:` - New features
  - `fix:` - Bug fixes
  - `refactor:` - Code refactoring
  - `docs:` - Documentation changes
  - `test:` - Adding or updating tests
  - `chore:` - Maintenance tasks

    Examples:
    ```
    feat: add job status polling to frontend
    fix: handle null response in job service
    refactor: extract message publishing to separate service
    docs: update README with deployment instructions
    ```

#### Code Review
- **Pull Request Reviews**: All code must be reviewed via PRs
- **Small PRs**: Keep PRs focused and small for easier review

#### Testing
- **To be build**
---

## 4. Explicitly Out of Scope (Deliberate Cuts)

These are **conscious scope decisions**, not omissions:
- Authentication / Authorization
- Redis or caching layers
- WebSockets / real‑time push
- Observability stack
- Infrastructure as Code
- Docker as a development requirement

---

## 5. What the System Does (End‑to‑End)

### High‑Level Consumption Flow

1. **UI** submits a job via `POST /jobs`
2. **API** validates input and creates a Job with status `QUEUED`
3. **API** publishes a minimal message to **SQS**
4. **SQS** stores the message durably
5. **Worker** consumes the message using long polling
6. **Worker** processes the job asynchronously
7. **Worker** updates Job status in **DynamoDB**
8. **UI** polls `GET /jobs/{id}` periodically
9. **API** reads from DynamoDB and returns current status

This full loop must work for the project to be considered delivered.

---

## 6. MVP – Non‑Negotiable Scope

### Backend
- `POST /jobs` – create a job
- `GET /jobs/{id}` – retrieve job status
- Publish messages to SQS
- Background worker consuming SQS
- Persist job status in DynamoDB

### Frontend
- Simple form to create a job
- Status view for a single job

### MVP Success Criteria
- Full async flow works end‑to‑end
- No manual intervention required

---

## 7. Architecture Layers

### Layer 1 – Core (Must Not Fail)
- API accepts job requests
- Message published to SQS
- Worker consumes message
- Processing visible via logs

### Layer 2 – Demonstrable Value
- Job status persisted in DynamoDB
- Status retrieval endpoint
- UI connected to API

### Layer 3 – Optional Extras (Only if Time Remains)
- Retry improvements
- Job listing per user
- Better UX polish

---

## 8. Roles & Ownership

### Daniel – Backend & Async Core
- Overall architecture
- .NET API
- SQS integration
- Worker implementation
- DynamoDB persistence
- Owner of Layer 1 (core reliability)

### Gabriel – Frontend & Integration
- Angular UI
- UI ↔ API integration
- User flow and basic UX
- Backend support when needed

**Rule:** each person owns their layer. Cross‑layer changes require alignment.

---

## 9. Scalability – Backend

### Throughput Targets
- Designed to handle **~1,000 requests/second** at peak
- Job creation: up to **~100 jobs/second**
- Status reads (polling): **~20–100 req/s**

### Dynamo DB Estimate ###
- Item size: ~1 KB
- Writes: ~200 writes/s (peak) ~200 WCU
- Reads: ~100 reads/s ~50 RCU
- Consistency: Eventual
- Transactions: none
- In develop we are limited to ≤ 25 WCU / 25 RCU

### SQS Characteristics
- Queue type: Standard
- Backlog capacity: effectively unlimited
- Delivery model: at‑least‑once
- Horizontal scaling handled by AWS

### Worker Model
- Single‑threaded per instance (simplicity first)
- Scale by running multiple workers
- Long polling (no fixed timers)

---

## 10. Scalability – Frontend

### Polling Strategy
- No WebSockets or push mechanisms
- Polling interval: **~5 seconds**
- Polling is aggregated per user, not per job

### Load Example
- 100 users → ~20 req/s
- 500 users → ~100 req/s

Frontend is not the bottleneck; API and storage are.

---

## 11. Cost Model (Order of Magnitude)

### Assumptions
- 1 job / second / active user
- 100 active users
- 30‑day month

### SQS – Job Submission
- Jobs/month: ~259 million
- Free tier: 1 million
- Billable: ~258 million
- Price: ~$0.40 per million requests

**Estimated cost (no batching):**
- ~103 USD / month

**With batching (10 messages per request):**
- ~26 million requests
- ~10 USD / month

### DynamoDB (On‑Demand)
- Reads: polling driven
- Writes: job creation + updates
- Expected cost: low tens of USD/month
- 100 users = 100 job/s == 100kb/s
- Job TTL: 6h ~2.1GB
- DynamoDB Physical Data Storage Cost (Monthly): 0.79 USD
- Monthly recording cost (Monthly): 246.38 USD
- Monthly reading cost (Monthly): 4.93 USD

### Cost Insight
- SQS is not the financial bottleneck
- Request efficiency matters more than raw volume

---

## 12. 4‑Day Execution Plan (Parallel Work)

### Day 1 – Feasibility & Skeleton

**Daniel**
- Bootstrap .NET Minimal API
- Configure AWS SDK access
- Create SQS queue
- Publish and consume test message (logs only)

**Gabriel**
- Bootstrap Angular project
- Create basic layout (form + status view)
- Align API contract with backend

**Success:** API → SQS → Worker visible in logs

---

### Day 2 – Functional Core

**Daniel**
- Create DynamoDB table
- Persist job status
- Implement POST /jobs and GET /jobs/{id}

**Gabriel**
- Implement job creation UI
- Connect UI to POST /jobs

**Success:** Full async flow works via API tools

---

### Day 3 – UI Integration & Stabilization

**Daniel**
- Basic error handling
- Minimal idempotency safeguards

**Gabriel**
- Implement polling
- Display job state transitions

**Success:** Demo works end‑to‑end without explanation

---

### Day 4 – Cleanup & Delivery

**Joint**
- Code cleanup
- README and setup instructions
- Demo narrative preparation

**Success:** A third party can run and understand the project

---

## 13. Recruiter Value Proposition

This project demonstrates:
- Async system design
- Cloud‑managed messaging
- Clear ownership boundaries
- Cost awareness
- Scalability reasoning
- Pragmatic scope control

BatchLab is not a tutorial. It is a **real engineering artifact delivered under constraints**.

## 14. Tools

- System designing: https://excalidraw.com/#json=5q1900ZLeO1h97Tdn99rS,YCkLMXxGnJbX4AlIcp7Y-Q
- SQS repo: https://github.com/aws/aws-sdk-net/tree/main/sdk/src/Services/SQS
- SQS code example: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/csharp_sqs_code_examples.html
- SQS code example: https://github.com/awsdocs/aws-doc-sdk-examples/tree/main/dotnetv3/SQS
- SQS setting up: https://docs.aws.amazon.com/en_us/AWSSimpleQueueService/latest/SQSDeveloperGuide/sqs-setting-up.html
- SQS api reference: https://docs.aws.amazon.com/AWSSimpleQueueService/latest/APIReference/Welcome.html
- SQS api doc: https://docs.aws.amazon.com/sdkfornet/v4/apidocs/items/SQS/NSQS.html
