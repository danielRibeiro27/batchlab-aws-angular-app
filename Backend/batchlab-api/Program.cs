var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/jobs", () => "Hello World!");
app.MapPost("/jobs", () => "Hello World!");

app.Run();
