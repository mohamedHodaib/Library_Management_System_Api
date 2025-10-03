using LibraryManagementSystem.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace LibraryManagementSystem.DataAccess
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Build configuration to read the connection string
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                //To obtain UserSecretsId
                .AddUserSecrets("99c77855-9703-4e91-81c5-9fc3a099b94e")
                .Build();

            // Create the DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("ConnectionString");

            optionsBuilder.UseSqlServer(connectionString);

            // Return a new instance of the DbContext
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}