# GitHub Copilot Instructions for BatchLab

This document provides guidelines for GitHub Copilot when working on the BatchLab repository.

## Project Overview

BatchLab is a cloud-native asynchronous batch processing system built with:
- **Backend**: .NET 10, ASP.NET Core Minimal API, AWS SDK for .NET
- **Frontend**: Angular 21 (standalone components)
- **Messaging**: AWS SQS (Standard Queue)
- **Persistence**: AWS DynamoDB (On-Demand)
- **Environment**: GitHub Codespaces

The system demonstrates production-grade async thinking through a simple end-to-end batch job processing flow.

## Build and Test Commands

### Backend (.NET)

**Important**: The backend consists of both an API and a Worker that consume from SQS. Both need to run simultaneously for the full async flow to work.

#### Running API
```bash
cd Backend/batchlab-api
dotnet restore
dotnet build
dotnet run
```

#### Running Worker
```bash
cd Backend/batchlab-api
dotnet run --worker  # Or separate worker project when available
```

**For development**, run both in separate terminal sessions:
- Terminal 1: API (`dotnet run`)
- Terminal 2: Worker (`dotnet run --worker`)

#### Testing
```bash
cd Backend/batchlab-api
dotnet test  # When tests are added
```

### Frontend (Angular)
```bash
cd Frontend
npm install
npm run build
npm start        # Runs ng serve
npm test         # Runs Vitest tests
```

## AWS Configuration

### Environment Variables

AWS credentials and configuration must be set individually for each environment. Required environment variables:

```bash
# AWS Credentials
AWS_ACCESS_KEY_ID=your_access_key_id
AWS_SECRET_ACCESS_KEY=your_secret_access_key
AWS_REGION=us-east-1  # Or your preferred region

# SQS Configuration
AWS_SQS_QUEUE_URL=https://sqs.{region}.amazonaws.com/{account-id}/{queue-name}

# DynamoDB Configuration
AWS_DYNAMODB_TABLE_NAME=Jobs
```

### Environment-Specific Configuration

For different environments (Development, Staging, Production), configure AWS credentials separately:

**Development (appsettings.Development.json or environment variables)**:
- Use development AWS account or local testing credentials
- Configure with appropriate IAM permissions for SQS and DynamoDB

**Production (appsettings.json or environment variables)**:
- Use production AWS account with restricted IAM permissions
- Follow principle of least privilege

### AWS SDK Configuration

When initializing AWS SDK services in code:
```csharp
// SQS Client
var sqsClient = new AmazonSQSClient(
    new BasicAWSCredentials(accessKey, secretKey),
    RegionEndpoint.GetBySystemName(region)
);

// DynamoDB Client
var dynamoDbClient = new AmazonDynamoDBClient(
    new BasicAWSCredentials(accessKey, secretKey),
    RegionEndpoint.GetBySystemName(region)
);
```

Or use default credential chain (recommended for Codespaces):
```csharp
var sqsClient = new AmazonSQSClient(RegionEndpoint.USEast1);
var dynamoDbClient = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
```

## General Coding Principles

### Commit Convention
- Follow Conventional Commits specification
- Use prefixes: `feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`
- Examples:
  - `feat: add job status polling to frontend`
  - `fix: handle null response in job service`
  - `refactor: extract message publishing to separate service`

### Code Review Standards
- Keep pull requests small and focused
- All code must be reviewed via PRs
- CI checks must pass before merge

### Scope Limitations
The following are **intentionally out of scope**:
- Authentication/Authorization
- Redis or caching layers
- WebSockets/real-time push
- Observability stack (logging is minimal)
- Infrastructure as Code
- Docker as development requirement

## Backend (.NET) Guidelines

### Naming Conventions
- **Classes, Interfaces, Methods (public), Properties**: `PascalCase`
  - Examples: `JobService`, `IJobRepository`, `CreateJobAsync`, `JobId`
- **Interfaces**: Prefix with `I`
  - Example: `IJobService`, `IMessagePublisher`
- **Variables (local), Parameters**: `camelCase`
  - Examples: `jobRequest`, `cancellationToken`, `jobId`

### Folder Structure
```
Backend/batchlab-api/
├── DTOs/                   # Data transfer objects
├── Domain/                 # Domain entities
├── Service/                # Business logic services
├── Repositories/           # Data access layer
├── Program.cs             # Application entry point
└── appsettings.json       # Configuration
```

### Code Standards
- **Use Minimal APIs**: Define endpoints using minimal API syntax
  ```csharp
  app.MapPost("/jobs", async (CreateJobRequest request, IJobService service) => 
      await service.CreateJobAsync(request));
  
  app.MapGet("/jobs/{id}", async (Guid id, IJobService service) =>
      await service.GetJobStatusAsync(id));
  ```

- **Dependency Injection**: Register all services in DI container in `Program.cs`
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

### Error Handling
- **Result Pattern**: Return results instead of throwing exceptions for expected errors
- **ProblemDetails**: Use standard ProblemDetails for API error responses
  ```csharp
  if (!validationResult.IsValid)
      return Results.ValidationProblem(validationResult.Errors);
  ```
- **Logging**: Always log with context (JobId, etc.)
  ```csharp
  _logger.LogInformation("Processing job {JobId}", jobId);
  _logger.LogError(ex, "Failed to process job {JobId}", jobId);
  ```

### AWS SQS Integration
- **Minimal Messages**: Keep messages small with only essential IDs
  ```csharp
  var message = new JobMessage(jobId);
  await _sqsPublisher.PublishAsync(message, cancellationToken);
  ```
- **JSON Serialization**: Use `System.Text.Json` for message serialization
- **Long Polling**: Workers should use long polling (WaitTimeSeconds = 20)

### AWS DynamoDB Integration
- **Naming**: Use PascalCase for table names and attributes
  - Table: `Jobs`, Attributes: `JobId`, `Status`, `CreatedAt`
- **Attributes**: Keep attribute names consistent with C# property names
  ```csharp
  var item = new Dictionary<string, AttributeValue>
  {
      ["JobId"] = new AttributeValue { S = job.Id.ToString() },
      ["Status"] = new AttributeValue { S = job.Status },
      ["CreatedAt"] = new AttributeValue { S = job.CreatedAt.ToString("o") }
  };
  ```

## Frontend (Angular) Guidelines

### Naming Conventions
- **Files and Selectors**: `kebab-case`
  - Examples: `job-form.component.ts`, `job-status.component.ts`
  - Selector: `app-job-form`, `app-job-status`
- **Classes and Interfaces**: `PascalCase`
  - Examples: `JobFormComponent`, `JobService`, `Job`, `JobStatusResponse`
- **Variables, Methods, Properties**: `camelCase`
  - Examples: `jobId`, `submitJob()`, `isLoading`

### Folder Structure
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

### Code Standards
- **Standalone Components**: Use standalone components (Angular 14+)
  ```typescript
  @Component({
    selector: 'app-job-form',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './job-form.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
  })
  export class JobFormComponent { }
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

### Services
- **ProvidedIn Root**: Services should be provided in root for singleton behavior
  ```typescript
  @Injectable({
    providedIn: 'root'
  })
  export class JobService { }
  ```

- **Single Responsibility**: Each service should have a single, well-defined purpose
- **Return Observables**: Service methods should return Observables
  ```typescript
  getJobStatus(id: string): Observable<JobStatusResponse> {
    return this.http.get<JobStatusResponse>(`/api/jobs/${id}`);
  }
  ```

### Components
- **One Component Per File**: Each component in its own file
- **Maximum 400 Lines**: Keep components under 400 lines; refactor if larger
- **Inline Templates**: Only use inline templates if less than 10 lines
  ```typescript
  // Inline template (only if < 10 lines)
  template: `<div>{{ job.status }}</div>`
  
  // Separate file (preferred for > 10 lines)
  templateUrl: './job-status.component.html'
  ```

### Polling Strategy
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

### TypeScript Standards
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

### Template Syntax
- **Use Angular 17+ Control Flow**: Prefer new control flow syntax
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

### Prettier Configuration
The project uses Prettier with these settings:
```json
{
  "printWidth": 100,
  "singleQuote": true
}
```

## Architecture Overview

### End-to-End Flow
1. **UI** submits a job via `POST /jobs`
2. **API** validates input and creates a Job with status `QUEUED`
3. **API** publishes a minimal message to **SQS**
4. **SQS** stores the message durably
5. **Worker** consumes the message using long polling
6. **Worker** processes the job asynchronously
7. **Worker** updates Job status in **DynamoDB**
8. **UI** polls `GET /jobs/{id}` periodically (every ~5 seconds)
9. **API** reads from DynamoDB and returns current status

### MVP Requirements
**Backend**:
- `POST /jobs` – create a batch job
- `GET /jobs/{id}` – retrieve job status
- Publish messages to SQS
- Background worker consuming SQS
- Persist job status in DynamoDB

**Frontend**:
- Simple form to create a job
- Status view for a single job with polling

### Scalability Considerations
- **Throughput**: Designed for ~1,000 requests/second at peak
- **Job creation**: Up to ~100 jobs/second
- **Status reads (polling)**: ~20–100 req/s
- **Worker model**: Single-threaded per instance, scale by running multiple workers
- **Polling interval**: ~5 seconds on frontend

## Testing Guidelines

### Backend Testing
- Use xUnit or NUnit for unit and integration tests
- Test critical paths:
  - Job creation flow
  - Status updates
  - Message publishing and consumption
  - Error handling scenarios
  
```csharp
[Fact]
public async Task CreateJobAsync_ShouldReturnJobId()
{
    // Arrange
    var request = new CreateJobRequest("TestJob", "{}");
    
    // Act
    var result = await _jobService.CreateJobAsync(request);
    
    // Assert
    Assert.NotEqual(Guid.Empty, result.JobId);
}
```

### Frontend Testing
- Use Vitest for unit tests
- Test critical components and services
  
```typescript
describe('JobService', () => {
  it('should create a job', async () => {
    const request = { name: 'Test', payload: '{}' };
    const response = await firstValueFrom(service.createJob(request));
    expect(response.id).toBeDefined();
  });
});
```

## Important Notes

- **Pragmatic Scope**: This is a 4-day delivery project focused on demonstrating async batch processing
- **Cloud-First**: Leverage AWS managed services (SQS, DynamoDB) over custom implementations
- **Simplicity**: Choose simple, working solutions over complex architecture
- **End-to-End**: Full async flow must work for the project to be considered delivered
- **No Authentication**: Authentication/authorization is intentionally out of scope
- **Codespaces Environment**: All development happens in GitHub Codespaces; no Docker required
