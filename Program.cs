using Microsoft.OpenApi.Models;
using MyProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração da string de conexão e nome do banco de dados
string mongoConnectionString = builder.Configuration.GetConnectionString("MongoDbConnection");
string databaseName = "MAODyO7UocjK3YTW"; // O nome do seu banco de dados

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Permite o frontend rodando em localhost:3000
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

builder.Services.AddSingleton<MongoDbService>(sp =>
    new MongoDbService(mongoConnectionString, databaseName));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });
});

var app = builder.Build();

// Use CORS
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = string.Empty; // faz o SwaggerUI ficar em http://localhost:5007/
});

app.MapControllers(); // <-- necessário para os controllers funcionarem

app.Run();