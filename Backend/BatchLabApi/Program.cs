using BatchLabApi.Dto;
using BatchLabApi.Service.Implementation;
using BatchLabApi.Service.Interface;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/jobs", () => "Hello World!");
app.MapPost("/jobs", async () =>
{
    JobApplicationService jobService = new JobApplicationService();
    JobDto job = new()
    {
        Desc = "HELLO_WORLD_JOB"
    };
    var result = await jobService.CreateAsync(job);
    return Results.Created("/jobs", new { status =  result});
});

app.Run();