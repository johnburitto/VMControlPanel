using Infrastructure.Services.Impls;
using Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;
using UserInfrastructure.Configurations;
using UserInfrastructure.Data;
using UserInfrastructure.Service.Imls;
using UserInfrastructure.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

// Add Serilog
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

// Add Redis caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Add Configurations
builder.Services.Configure<TokenGenerateServiceConfiguration>(options => builder.Configuration.GetSection("TokenGenerateServiceConfiguration").Bind(options));

// Add Dependencies
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenGenerateService, TokenGenerateService>();
builder.Services.AddScoped<ICacheService, CacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
