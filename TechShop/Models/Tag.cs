using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class Tag:BaseEntity
    {
        [MaxLength(20)]
        public string Name { get; set; }
    }
}
