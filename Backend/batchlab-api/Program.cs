using BatchlabApi.Dto;
using BatchlabApi.Service.Implementation;
using BatchlabApi.Service.Interface;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/jobs", () => "Hello World!");
app.MapPost("/jobs", async () =>
{
    JobApplicationService jobService = new JobApplicationService(new SQSMessageBus());
    JobDto job = new JobDto
    {
        Id = 1,
        Desc = "Sample Job"
    };
    var result = await jobService.CreateAsync(job);
    return Results.Created("/jobs/1", new { id = 1, status =  result});
});

app.Run();