using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EMGADSB.Services
{
    public static class JwtConfigurationExtensions
    {
        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            // Récupération de la configuration JWT à partir de appsettings.json
            var jwtSettings = configuration.GetSection("JWT"); 

            // Utilisation de la clé secrète définie dans le fichier appsettings.json
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]); 

            // Configuration de l'authentification JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],  // Vérification de l'émetteur
                    ValidAudience = jwtSettings["Audience"],  // Vérification de l'audience
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero  // Pas de délai supplémentaire pour l'expiration du jeton
                };
            });
        }
    }
}
