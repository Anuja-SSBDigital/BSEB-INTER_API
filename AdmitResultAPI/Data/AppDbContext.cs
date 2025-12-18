
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
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);
        }
    }
}
