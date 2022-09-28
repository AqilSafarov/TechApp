using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class AppUser:IdentityUser
    {
        public string Fullname { get; set; }
        public bool IsMember { get; set; }
        public bool IsActive { get; set; }

    }
}
