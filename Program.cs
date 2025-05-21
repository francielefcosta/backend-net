using Microsoft.OpenApi.Models;
using MyProject.Services;


DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

string? mongoConnectionString = builder.Configuration.GetConnectionString("MongoDbConnection");
string databaseName = "database_dev_fran";

if (string.IsNullOrEmpty(mongoConnectionString))
{
    throw new Exception("A connection string MongoDbConnection não foi encontrada!");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

builder.Services.AddSingleton<MongoDbService>(sp =>
    new MongoDbService(mongoConnectionString, databaseName));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
     c.RoutePrefix = string.Empty; 
});

app.MapControllers();

app.Run();