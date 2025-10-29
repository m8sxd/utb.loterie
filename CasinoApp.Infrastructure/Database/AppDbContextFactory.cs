using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using System;

namespace CasinoApp.Infrastructure.Database
{
    // Tato třída říká nástrojům (jako 'Add-Migration'), jak vytvořit AppDbContext
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 34)); 

            string connectionString = "Server=localhost;Port=3306;Database=casino_db_design;User=root;Password=;";

            optionsBuilder.UseMySql(connectionString, serverVersion);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}