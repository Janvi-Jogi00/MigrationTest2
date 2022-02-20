using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class MigrationContext : DbContext
    {
        public DbSet<SourceTable> SourceTable { get; set; }
        public DbSet<DestinationTable> DestinationTable { get; set; }
        public DbSet<MigrationStatusTable> MigrationStatus { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDb; Initial Catalog=TestMigrationDB ");
        }
    }
}
