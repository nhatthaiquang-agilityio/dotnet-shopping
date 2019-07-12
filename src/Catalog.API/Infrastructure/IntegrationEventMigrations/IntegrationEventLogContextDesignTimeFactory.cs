using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using BuildingBlocks.IntegrationEventLogEF;
using System;

namespace Catalog.API.Infrastructure
{
    public class IntegrationEventLogContextDesignTimeFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
    {
       

        public IntegrationEventLogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>();

            optionsBuilder.UseSqlServer("Server=localhost;Database=catalogdb;User Id=sa;Password=Your_password123;",
                options => options.MigrationsAssembly(GetType().Assembly.GetName().Name));

            Console.WriteLine("Migrations: event");
            Console.WriteLine(GetType().Assembly.GetName().Name);

            return new IntegrationEventLogContext(optionsBuilder.Options);
        }
    }
}