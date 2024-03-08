using Microsoft.EntityFrameworkCore;
using MvcProyectoPerfumes.Models;

namespace MvcProyectoPerfumes.Data
{
    public class PerfumesContext: DbContext
    {
        public PerfumesContext(DbContextOptions<PerfumesContext> options):base(options) { }

        public DbSet<Perfume> Perfumes { get; set; }
        public DbSet<NotaOlor> NotasOlor { get; set; }
        public DbSet<PerfumeNotaOlor> PerfumeNotaOlor { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PerfumeNotaOlor>()
            .HasKey(pno => new { pno.PerfumeID, pno.NotaOlorID });

            //modelBuilder.Entity<PerfumeNotaOlor>()
            //    .HasOne(pno => pno.Perfume)
            //    .WithMany(p => p.PerfumeNotaOlor)
            //    .HasForeignKey(pno => pno.PerfumeID);

            //modelBuilder.Entity<PerfumeNotaOlor>()
            //    .HasOne(pno => pno.NotaOlor)
            //    .WithMany()
            //    .HasForeignKey(pno => pno.NotaOlorID);
            //base.OnModelCreating(modelBuilder);
        }
    }
}
