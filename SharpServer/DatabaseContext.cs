using Microsoft.EntityFrameworkCore;
using System;

namespace SharpServer
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Models.Message> Messages { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") == null
                ? "Server=host.docker.internal;Port=3306;Database=messenger_database;Uid=root;Pwd=no_piko;" :
                Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            optionsBuilder.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(8, 0, 29)),
                    options => options.EnableRetryOnFailure(3)
                );
        }
    }
}
