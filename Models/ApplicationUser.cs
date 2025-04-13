using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EMGADSB.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Propriétés supplémentaires si nécessaires
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    
}