using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TechMoves.API.Data;
using TechMoves.API.Models;
using TechMoves.API.Repositories;
using TechMoves.API.Services;
using TechMoves.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON fix
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// =========================
// FIXED DEPENDENCY INJECTION
// =========================
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<ContractRepository>();

// IMPORTANT: Currency service + HttpClient
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }