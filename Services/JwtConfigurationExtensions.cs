using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace EMGADSB.Services
{
    public static class JwtConfigurationExtensions
    {
        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            // Récupération de la configuration JWT à partir de appsettings.json
            var jwtSettings = configuration.GetSection("JwtSettings");

            // Enregistrer l'objet JwtSettings pour l'injection de dépendances
            services.Configure<JwtSettings>(jwtSettings);

            // Utilisation de la clé secrète définie dans le fichier appsettings.json
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

            // Configuration de l'authentification JWT
            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero  // Pas de délai supplémentaire pour l'expiration du jeton
                    };
                });
        }
    }
}