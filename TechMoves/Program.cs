using Microsoft.EntityFrameworkCore;
using TechMoves.Data;
using TechMoves.Interfaces;
using TechMoves.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// BUSINESS SERVICES 
builder.Services.AddScoped<IContractService, ContractService>();

// EXTERNAL API SERVICE 
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();