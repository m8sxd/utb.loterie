using CasinoApp.Application.Interfaces;
using CasinoApp.Application.Services;
using CasinoApp.Infrastructure.Database;
using CasinoApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "UTBCasino.Auth";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var serverVersion = new MariaDbServerVersion(new Version(10, 4, 28)); 

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, b =>
    {
        b.EnableRetryOnFailure();
    })
);

builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); 
builder.Services.AddScoped<IBetRepository, BetRepository>();   
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddScoped<IBettingService, BettingService>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IBlackjackGameRepository, BlackjackGameRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error"); 
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Do�lo k chyb� p�i migraci datab�ze.");
    }
}

app.Run();