using CollegeEventsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeEventsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Registration> Registrations => Set<Registration>();
        public DbSet<Venue> Venues => Set<Venue>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Registrations)
                .WithOne(r => r.Student)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

           
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Registrations)
                .WithOne(r => r.Event)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Registration>()
                .HasIndex(r => new { r.StudentId, r.EventId })
                .IsUnique();
            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Events)
                .WithOne(e => e.Venue)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Events)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
