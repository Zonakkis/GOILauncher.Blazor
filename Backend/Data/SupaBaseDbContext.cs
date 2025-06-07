using Microsoft.EntityFrameworkCore;
using Shared.Models.Entities;

namespace Backend.Data
{
    public class SupaBaseDbContext : DbContext
    {
        public DbSet<Speedrun> Speedruns { get; set; }
        public DbSet<SpeedrunsJson> SpeedrunsJson { get; set; }

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
            modelBuilder.Entity<SpeedrunsJson>(entity =>
            {
                entity.ToTable("speedruns_json");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Category).HasColumnName("category").HasConversion<string>();
                entity.Property(e => e.Platform).HasColumnName("platform").HasConversion<string>();
                entity.Property(e => e.Json).HasColumnName("json");
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=db.rnpvypikpxfrqgenfhkj.supabase.co;Port=5432;Username=postgres;Password=qqshabizwqqq;Database=postgres;Ssl Mode=Require;Trust Server Certificate=true");
        }
    }
}
