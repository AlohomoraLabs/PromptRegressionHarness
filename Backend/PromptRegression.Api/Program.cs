using PromptRegression.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient<ILlmService, OllamaLlmService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
});
builder.Services.AddScoped<EmailEvaluator>();
builder.Services.AddScoped<RegressionService>();
builder.Services.AddScoped<HallucinationEvaluator>();
builder.Services.AddScoped<ToneEvaluator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
