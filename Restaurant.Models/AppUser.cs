
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Restaurant.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
    }
    }
