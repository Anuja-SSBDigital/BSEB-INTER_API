
using AdmitResultAPI.Model;
using AdmitResultAPI.Model.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AdmitResultAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DummyAdmitCardDto> DummyAdmitCardDto { get; set; }

        public DbSet<SubjectPaperDto> SubjectPaperEntities { get; set; }
        public DbSet<SpResultDto> SpResultDto { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SubjectPaperDto>().HasNoKey();
            modelBuilder.Entity<DummyAdmitCardDto>().HasNoKey();
            modelBuilder.Entity<SpResultDto>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}
