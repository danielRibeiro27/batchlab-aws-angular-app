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
app.MapGet("/jobs", () => "Hello World!");
app.MapPost("/jobs", async ([FromBody]JobDto jobDto, IJobApplicationService _jobService) =>
{
    if(jobDto == null)
        return Results.BadRequest("Job data is required.");
    
    //TO-DO: Inject service
    //TO-DO: Add error handling
    //TO-DO: Validate job data
    //TO-DO: Return proper response with job id or status
    //TO-DO: Log the request and response
    //TO-DO: Create mapper between JobDto and JobEntity
    JobEntity jobEntity = new JobEntity().New(jobDto.Description);
    var result = await _jobService.CreateAsync(jobEntity);
    if(!result)
        return Results.StatusCode(500);

    return Results.Created("/jobs", null);
});

#endregion

app.Run();