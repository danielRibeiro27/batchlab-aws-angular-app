using System.Text.Json;
using BatchLabApi.Domain;
using BatchLabApi.Dto;
using BatchLabApi.Extensions;
using BatchLabApi.Service.Implementation;
using BatchLabApi.Service.Interface;
using Microsoft.AspNetCore.Mvc;


#region Initial Setup

var builder = WebApplication.CreateBuilder(args);

#region DI
builder.Services.AddJobApplicationServices();
builder.Services.AddJobApplicationInfrastructures();
#endregion

var app = builder.Build();
#endregion

#region Endpoints
app.MapGet("/jobs/{id}", async (string id, IJobApplicationService _jobService) =>
{
    var jobEntity = await _jobService.GetByIdAsync(id);
    if(jobEntity == null)
        return Results.NotFound();

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
    foreach (var jobEntity in jobEntities)
    {
        jobsDto.Add(new JobDto
        {
            Id = jobEntity.Id.ToString(),
            Description = jobEntity.Description,
            Status = jobEntity.Status,
            CreatedAt = jobEntity.CreatedAt
        });
    }

    return Results.Ok(jobsDto);
});

app.MapPost("/jobs", async Task<IResult> ([FromBody]JobDto jobDto, IJobApplicationService _jobService) =>
{
    if(jobDto == null)
        return Results.BadRequest("Job data is required.");
    
    //TO-DO: Add error handling
    //TO-DO: Validate job data
    //TO-DO: Return proper response with job id or status
    //TO-DO: Log the request and response
    //TO-DO: Create mapper between JobDto and JobEntity
    JobEntity jobEntity = new(jobDto.Description!);
    var result = await _jobService.PublishAsync(jobEntity);
    if(!result)
        return Results.StatusCode(500);

    var createdJobDto = new JobDto
    {
        Id = jobEntity.Id.ToString(),
        Description = jobEntity.Description,
        Status = jobEntity.Status,
        CreatedAt = jobEntity.CreatedAt
    };

    return Results.Created($"/jobs/{createdJobDto.Id}", createdJobDto);
});

#endregion

app.Run();