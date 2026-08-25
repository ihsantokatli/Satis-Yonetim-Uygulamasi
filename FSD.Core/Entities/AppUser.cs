using Microsoft.AspNetCore.Identity;
using System;

namespace FSD.Core.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public UserType UserType { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}