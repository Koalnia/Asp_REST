using Asp_REST.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Asp_REST.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Advertisements)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
        .        OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Role)
                      .HasConversion<string>();
            });  
        }
    }
}
