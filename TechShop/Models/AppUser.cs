using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class AppUser:IdentityUser
    {
        public string Fullname { get; set; }
        public bool IsMember { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        [MaxLength(20), DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [NotMapped]
        [MaxLength(20),DataType(DataType.Password)]

        public string Password { get; set; }
        [NotMapped]
        [MaxLength(20), DataType(DataType.Password),Compare(nameof(Password))]

        public string ConfirmPassword { get; set; }
        public List<Order> Orders { get; set; }

    }
}
