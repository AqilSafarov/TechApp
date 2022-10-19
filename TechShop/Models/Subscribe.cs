using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class Subscribe:BaseEntity
    {
        [Required, MaxLength(100)]
        public string Email { get; set; }
    }
}
