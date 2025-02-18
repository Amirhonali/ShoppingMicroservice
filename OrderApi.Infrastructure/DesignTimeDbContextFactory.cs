using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using OrderApi.Infrastructure.Data;

namespace Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../OrderApi.Presentation"))
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("eCommerceConnection");

            // Npgsql bilan Postgresql ga ulanish
            optionsBuilder.UseNpgsql(connectionString);

            return new OrderDbContext(optionsBuilder.Options);
        }
    }
}