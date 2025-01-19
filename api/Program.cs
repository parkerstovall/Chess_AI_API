using api.BackgroundJobs;
using api.models.db;
using api.repository;
using Middleware;

DatabaseInit.RegisterModels();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<GameRepository>();
builder.Services.AddScoped<ConnectionRepository>();
builder.Services.AddHostedService<DeactivateGame>();
builder.Services.AddHttpContextAccessor();

//Add configuration files
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var configBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var config = configBuilder.Build();
builder
    .Services.AddOptions<Dictionary<string, DatabaseSettings>>()
    .Bind(config.GetSection("Collections"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowApp",
        builder =>
        {
            builder
                .WithOrigins("https://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorMiddleware>();

app.UseCors("AllowApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
