using CasinoApp.Application.Interfaces;
using CasinoApp.Application.Services;
using CasinoApp.Infrastructure.Database; // Odkaz na AppDbContext
using CasinoApp.Infrastructure.Repositories; // Odkaz na implementace
using Microsoft.EntityFrameworkCore;
using System.Reflection; // Pro Swagger XML

var builder = WebApplication.CreateBuilder(args);

// --- Konfigurace Slu�eb (Service Container) ---

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Povolen� XML dokumentace pro Swagger
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Datab�ze (pou��v� SQLite)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseMySql(connectionString, serverVersion)
);

// Registrace Aplika�n�ch a Infrastrukturn�ch Slu�eb
// Registrace Repozit���
builder.Services.AddScoped<IWalletRepository, WalletRepository>();

// Registrace N�STROJ� pro architektonickou �istotu
builder.Services.AddScoped<ITransactionManager, TransactionManager>();

// Registrace Aplika�n�ch slu�eb (Business Logika)
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