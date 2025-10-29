using CasinoApp.Application.Interfaces;
using CasinoApp.Application.Services;
using CasinoApp.Infrastructure.Database; // Odkaz na AppDbContext
using CasinoApp.Infrastructure.Repositories; // Odkaz na implementace
using Microsoft.EntityFrameworkCore;
using System.Reflection; // Pro Swagger XML

var builder = WebApplication.CreateBuilder(args);

// --- Konfigurace Služeb (Service Container) ---

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Povolení XML dokumentace pro Swagger
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Databáze (používá SQLite)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseMySql(connectionString, serverVersion)
);

// Registrace Aplikaèních a Infrastrukturních Služeb
// Registrace Repozitáøù
builder.Services.AddScoped<IWalletRepository, WalletRepository>();

// Registrace NÁSTROJÙ pro architektonickou èistotu
builder.Services.AddScoped<ITransactionManager, TransactionManager>();

// Registrace Aplikaèních služeb (Business Logika)
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<IBettingService, BettingService>();

var app = builder.Build();

// --- Konfigurace HTTP Request Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// app.UseAuthentication(); 
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();