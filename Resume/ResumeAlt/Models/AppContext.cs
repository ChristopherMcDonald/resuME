using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Resume.Models
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>();
            modelBuilder.Entity<Favourite>().HasKey(entity => new { entity.TemplateId, entity.UserId });
        }
    }
}
