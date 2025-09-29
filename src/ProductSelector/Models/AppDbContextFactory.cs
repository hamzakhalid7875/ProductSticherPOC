using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProductSelector.Models;
using System.IO;

namespace ProductSelector.Data
{
    // This ensures EF tools can build the DbContext at design time and read appsettings.json
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Set base path to the project directory where appsettings.json is located
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var conn = config.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("DefaultConnection not found in appsettings.json");

            optionsBuilder.UseSqlServer(conn, sql => sql.EnableRetryOnFailure());

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
