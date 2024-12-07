using Microsoft.EntityFrameworkCore;
using vertical_slice.Entities;

namespace vertical_slice.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Article>(builder =>
            //    builder.OwnsOne(a=> a.))
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Article> Articles { get; set; }
    }
}
