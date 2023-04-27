using CtaLinea.Model.Runs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Model.Runs;

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

        public DbSet<RunItem> Runs { get; set; }
        */

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                this._config.ConnectionString
                );
        }

        public IDbConnection GetNewConnection ()
        {
            var conn = new SqlConnection(this.Database.GetConnectionString());
            return conn;
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            // table names
            /*
            modelBuilder.Entity<RunItem>(e => e.ToTable("Runs"));
            modelBuilder.Entity<RunItem>()
                .HasKey(d => d.RunId);


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
