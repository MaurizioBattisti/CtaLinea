using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Context
{
    public class CtaDbContext
        : DbContext
    {
        private readonly  CtaLineaDbContextConfiguration _config;

        public CtaDbContext (
            CtaLineaDbContextConfiguration configuration)
        {
            this._config = configuration;
        }

        /*
        public DbSet<Associate> Associates { get; set; }
        public DbSet<Car> Cars { get; set; }
        */

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                this._config.ConnectionString
                );
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            // table names
            /*
            modelBuilder.Entity<Associate>(e => e.ToTable("Associate"));
            modelBuilder.Entity<Car>(e => e.ToTable("Car"));

            modelBuilder.Entity<Associate>()
                .HasKey(d => d.AssociateID);

            modelBuilder.Entity<Car>()
                .HasKey(d => d.CarID);
            */
        }
    }
}
