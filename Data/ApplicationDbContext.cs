using EMGADSB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EMGADSB.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<CarMake> CarMakes { get; set; }
        public DbSet<CarModel> CarModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // ⚠️ Important de le laisser pour Identity

            // Configuration des relations entre Car et CarMake
            modelBuilder.Entity<Car>()
                .HasOne(c => c.CarMakeNavigation)
                .WithMany(m => m.Cars)
                .HasForeignKey(c => c.CarMakeId);

            // Configuration des relations entre CarModel et CarMake
            modelBuilder.Entity<CarModel>()
                .HasOne(m => m.CarMakeNavigation)
                .WithMany(m => m.CarModels)
                .HasForeignKey(m => m.CarMakeId);

            // Configuration de la propriété "Year" de la table Car
            modelBuilder.Entity<Car>()
                .Property(c => c.Year)
                .HasAnnotation("MinValue", 2018);

            // Configuration des colonnes "PurchasePrice" et "SellingPrice" de type decimal
            modelBuilder.Entity<Car>()
                .Property(c => c.PurchasePrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Car>()
                .Property(c => c.SellingPrice)
                .HasColumnType("decimal(18, 2)");

            // Insertion des données de base pour CarMake
            modelBuilder.Entity<CarMake>().HasData(
                new CarMake { Id = 1, Name = "Toyota" },
                new CarMake { Id = 2, Name = "Honda" },
                new CarMake { Id = 3, Name = "Ford" },
                new CarMake { Id = 4, Name = "BMW" },
                new CarMake { Id = 5, Name = "Mercedes-Benz" },
                new CarMake { Id = 6, Name = "Audi" },
                new CarMake { Id = 7, Name = "Volkswagen" },
                new CarMake { Id = 8, Name = "Hyundai" },
                new CarMake { Id = 9, Name = "Kia" },
                new CarMake { Id = 10, Name = "Nissan" }
            );

            // Insertion des données de base pour CarModel
            modelBuilder.Entity<CarModel>().HasData(
                new CarModel { Id = 1, Name = "Corolla", CarMakeId = 1 },
                new CarModel { Id = 2, Name = "Camry", CarMakeId = 1 },
                new CarModel { Id = 3, Name = "RAV4", CarMakeId = 1 },
                new CarModel { Id = 4, Name = "Civic", CarMakeId = 2 },
                new CarModel { Id = 5, Name = "Accord", CarMakeId = 2 },
                new CarModel { Id = 6, Name = "CR-V", CarMakeId = 2 },
                new CarModel { Id = 7, Name = "Focus", CarMakeId = 3 },
                new CarModel { Id = 8, Name = "Mustang", CarMakeId = 3 },
                new CarModel { Id = 9, Name = "F-150", CarMakeId = 3 }
            );

            // ----- Seed roles -----
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "User",
                    NormalizedName = "USER"
                }
            );

            // ----- Seed admin user -----
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = "1",
                UserName = "admin@emgoasb.com",
                NormalizedUserName = "ADMIN@EMGOASB.COM",
                Email = "admin@emgoasb.com",
                NormalizedEmail = "ADMIN@EMGOASB.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(), // Meilleur que string.Empty
                FirstName = "Admin",
                LastName = "Principal"
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            // ----- Assign admin role to user -----
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "1",
                    UserId = "1"
                }
            );
        }
    }
}
