using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class Slider:BaseEntity
    {
        [MaxLength(150)]
        public string Title { get; set; }
        [MaxLength(350)]

        public string Text { get; set; }
        [Column(TypeName = "float")]
        public double? Price { get; set; }
        [MaxLength(350)]

        public string RedirectUrl { get; set; }
        [MaxLength(250)]

        public string Photo { get; set; }
    }
}
