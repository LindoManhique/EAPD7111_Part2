using TechMoves.Services;
using TechMoves.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// HTTP CLIENT (CONNECT TO API)
builder.Services.AddHttpClient<ApiClientService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7009/");
});

// BUSINESS SERVICES 
builder.Services.AddScoped<IContractService, ContractService>();

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