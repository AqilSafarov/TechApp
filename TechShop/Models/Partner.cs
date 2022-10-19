using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class Partner:BaseEntity
    {
        [MaxLength(250)]
        public string Photo { get; set; }
        [Range(1, int.MaxValue)]

        public int Order { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
