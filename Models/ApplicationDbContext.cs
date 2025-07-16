using Microsoft.EntityFrameworkCore;

namespace ResourceBookingSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure one-to-many relationship between Resource and Booking
            // Each Booking is for one Resource; a Resource can have many Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Resource)
                .WithMany()
                .HasForeignKey(b => b.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        // Seed demo data for Resources if table is empty
        public static void SeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                // Only seed if there are no resources in the database
                if (!context.Resources.Any())
                {
                    context.Resources.AddRange(
                        new Resource
                        {
                            Name = "Meeting Room Alpha",
                            Description = "Large room with projector and whiteboard",
                            Location = "3rd Floor, West Wing",
                            Capacity = 10,
                            IsAvailable = true
                        },
                        new Resource
                        {
                            Name = "Company Car 1",
                            Description = "Compact sedan",
                            Location = "Parking Bay 5",
                            Capacity = 4,
                            IsAvailable = true
                        },
                        new Resource
                        {
                            Name = "Conference Hall",
                            Description = "Spacious hall for large events",
                            Location = "Ground Floor",
                            Capacity = 50,
                            IsAvailable = true
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
} 