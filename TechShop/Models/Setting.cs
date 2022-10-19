using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class Setting:BaseEntity
    {
        [MaxLength(250)]
        public string LogoNav { get; set; }
        [NotMapped]
        public IFormFile ImageFileNav { get; set; }

        [MaxLength(250)]
        public string LogoFooter { get; set; }
        [NotMapped]
        public IFormFile ImageFileFooter { get; set; }
        [MaxLength(500)]
        public string About { get; set; }
        [MaxLength(50)]

        public string PhoneNumNav { get; set; }
        [MaxLength(100)]

        public string Address { get; set; }
        [MaxLength(100)]

        public string WorkStart { get; set; }
        [MaxLength(100)]

        public string WorkClose { get; set; }
        [MaxLength(100)]

        public string EmailNav { get; set; }
        [MaxLength(100)]

        public string EmailContact { get; set; }
        [MaxLength(100)]

        public string ContactNumPhone { get; set; }

    }
}
