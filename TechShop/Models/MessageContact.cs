using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class MessageContact:BaseEntity
    {
        public string Fullname { get; set; }
        [Required, MaxLength(100)]
        public string Email { get; set; }
        [Required, MaxLength(100)]
        public string Subject { get; set; }
        [Required, MaxLength(500)]
        public string Text { get; set; }
    }
}
