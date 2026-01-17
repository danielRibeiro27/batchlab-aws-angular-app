using Amazon.DynamoDBv2;
using Amazon.SQS;
using BatchLabApi.Domain;
using BatchLabApi.Dto;
using BatchLabApi.Extensions;
using BatchLabApi.Service.Interface;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

#region Initial Setup

var builder = WebApplication.CreateBuilder(args);

#region DI
builder.Services.AddJobApplicationServices();
builder.Services.AddJobApplicationInfrastructures();
#endregion

var app = builder.Build();

#region Global Exception Handler
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/json";

        var (statusCode, errorMessage) = exception switch
        {
            ArgumentException ex => (400, ex.Message),
            AmazonDynamoDBException ex => (500, "DynamoDB error: " + ex.Message),
            AmazonSQSException ex => (500, "SQS error: " + ex.Message),
            KeyNotFoundException ex => (500, "Data mapping error: " + ex.Message),
            FormatException ex => (500, "Data corruption detected: " + ex.Message),
            _ => (500, "An unexpected error occurred")
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { error = errorMessage });
    });
});
#endregion

#region Endpoints
app.MapGet("/jobs/{id}", async (string id, IJobApplicationService _jobService) =>
{
    if(string.IsNullOrEmpty(id))
        return Results.BadRequest("Job ID is required.");

    var jobEntity = await _jobService.GetByIdAsync(id);
    if(jobEntity == null) return Results.NotFound();

    var jobDto = new JobDto
    {
        Id = jobEntity.Id.ToString(),
        Description = jobEntity.Description,
        Status = jobEntity.Status,
        CreatedAt = jobEntity.CreatedAt
    };

    return Results.Ok(jobDto);
});

app.MapGet("/jobs", async (IJobApplicationService _jobService) =>
{
    var jobEntities = await _jobService.GetAllAsync();
    if(jobEntities == null)
        return Results.NotFound();

    // TODO: Refactor to use LINQ Select for more concise and idiomatic transformation
    // Example: jobEntities.Select(e => new JobDto { Id = e.Id.ToString(), Description = e.Description, Status = e.Status, CreatedAt = e.CreatedAt }).ToList()
    var jobsDto = new List<JobDto>();
    jobsDto.AddRange(jobEntities.Select(e => new JobDto
    {
        Id = e.Id.ToString(),
        Description = e.Description,
        Status = e.Status,
        CreatedAt = e.CreatedAt
    }));

    return Results.Ok(jobsDto);
});

app.MapPost("/jobs", async Task<IResult> ([FromBody]JobDto jobDto, IJobApplicationService _jobService) =>
{
    if(jobDto == null)
        return Results.BadRequest("Job data is required.");
    
    //TO-DO: Create mapper between JobDto and JobEntity
    JobEntity jobEntity = new(jobDto.Description!);
    var result = await _jobService.PublishAsync(jobEntity);
    if(!result)
        return Results.Json(new { error = "Failed to publish job to message bus" }, statusCode: 500);

    return Results.Created($"/jobs/{jobEntity.Id}", jobEntity);
});

#endregion

#endregion

app.Run();