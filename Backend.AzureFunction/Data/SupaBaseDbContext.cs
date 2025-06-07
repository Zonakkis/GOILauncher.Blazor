using Microsoft.EntityFrameworkCore;
using Shared.Models.Entities;

namespace Backend.Data
{
    public class SupaBaseDbContext(DbContextOptions<SupaBaseDbContext> options) : DbContext(options)
    {
        public DbSet<Speedrun> Speedruns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Speedrun>(entity =>
            {
                entity.ToTable("speedruns");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Player).HasColumnName("player");
                entity.Property(e => e.Area).HasColumnName("area");
                entity.Property(e => e.Category).HasColumnName("category");
                entity.Property(e => e.Platform).HasColumnName("platform");
                entity.Property(e => e.Time).HasColumnName("time");
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.Comment).HasColumnName("comment");
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=db.rnpvypikpxfrqgenfhkj.supabase.co;Port=5432;Username=postgres;Password=qqshabizwqqq;Database=postgres;Ssl Mode=Require;Trust Server Certificate=true");
        }
    }
}
