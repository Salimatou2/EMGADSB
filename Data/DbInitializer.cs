using EMGADSB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EMGADSB.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                // Migrations
                context.Database.Migrate();
                logger.LogInformation("Migrations appliquées avec succès");

                // Création des rôles
                string[] roleNames = { "Admin", "User" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                        logger.LogInformation($"Rôle {roleName} créé avec succès");
                    }
                }

                // Création de l'admin par défaut
                var adminEmail = "admin@emgvoitures.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "Admin",
                        LastName = "EMG",
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(adminUser, "Admin123!");
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Utilisateur admin créé avec succès");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.LogInformation("Utilisateur admin ajouté au rôle Admin");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            logger.LogError($"Erreur lors de la création de l'admin: {error.Description}");
                        }
                    }
                }

                // Ajouter des marques de voitures si la table est vide
                if (!context.CarMakes.Any())
                {
                    var makes = new[]
                    {
                        new CarMake { Name = "Toyota" },
                        new CarMake { Name = "Honda" },
                        new CarMake { Name = "BMW" },
                        new CarMake { Name = "Mercedes" },
                        new CarMake { Name = "Audi" },
                        new CarMake { Name = "Ford" },
                        new CarMake { Name = "Volkswagen" }
                    };
                    context.CarMakes.AddRange(makes);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Marques de voitures ajoutées avec succès");
                }

                // Initialisation des modèles pour chaque marque
                await InitializeCarModels(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de données");
                throw;
            }
        }

        private static async Task InitializeCarModels(ApplicationDbContext context, ILogger logger)
        {
            // Définition des modèles pour chaque marque
            var carMakesAndModels = new Dictionary<string, List<string>>
            {
                {
                    "Toyota", new List<string>
                    {
                        "Corolla", "Camry", "RAV4", "Highlander", "Yaris",
                        "Prius", "C-HR", "Land Cruiser", "Avensis", "Aygo"
                    }
                },
                {
                    "Honda", new List<string>
                    {
                        "Civic", "Accord", "CR-V", "HR-V", "Jazz",
                        "Pilot", "Insight", "Odyssey", "City", "Fit"
                    }
                },
                {
                    "BMW", new List<string>
                    {
                        "Série 1", "Série 2", "Série 3", "Série 4", "Série 5",
                        "Série 7", "X1", "X3", "X5", "X6", "Z4", "i3", "i8"
                    }
                },
                {
                    "Mercedes", new List<string>
                    {
                        "Classe A", "Classe B", "Classe C", "Classe E", "Classe S",
                        "GLA", "GLB", "GLC", "GLE", "GLS", "CLA", "CLS", "EQA", "EQC"
                    }
                },
                {
                    "Audi", new List<string>
                    {
                        "A1", "A3", "A4", "A5", "A6", "A7", "A8",
                        "Q2", "Q3", "Q5", "Q7", "Q8", "e-tron", "TT", "R8"
                    }
                },
                {
                    "Ford", new List<string>
                    {
                        "Fiesta", "Focus", "Mustang", "Kuga", "Puma", "EcoSport",
                        "Explorer", "Ranger", "Transit", "Bronco", "Edge", "F-150"
                    }
                },
                {
                    "Volkswagen", new List<string>
                    {
                        "Polo", "Golf", "Passat", "Tiguan", "Touareg", "Arteon",
                        "T-Cross", "T-Roc", "ID.3", "ID.4", "Caddy", "Transporter"
                    }
                }
            };

            // Pour chaque marque, ajouter les modèles s'ils n'existent pas déjà
            foreach (var makeName in carMakesAndModels.Keys)
            {
                var make = await context.CarMakes.FirstOrDefaultAsync(m => m.Name == makeName);
                if (make != null)
                {
                    // Récupérer les modèles existants pour cette marque
                    var existingModels = await context.CarModels
                        .Where(m => m.CarMakeId == make.Id)
                        .Select(m => m.Name)
                        .ToListAsync();

                    // Ajouter uniquement les modèles qui n'existent pas encore
                    var modelsToAdd = carMakesAndModels[makeName]
                        .Where(modelName => !existingModels.Contains(modelName))
                        .Select(modelName => new CarModel { Name = modelName, CarMakeId = make.Id })
                        .ToList();

                    if (modelsToAdd.Any())
                    {
                        context.CarModels.AddRange(modelsToAdd);
                        await context.SaveChangesAsync();
                        logger.LogInformation($"{modelsToAdd.Count} nouveaux modèles ajoutés pour {makeName}");
                    }
                }
            }
        }
    }
}