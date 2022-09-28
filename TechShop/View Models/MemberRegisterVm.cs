using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.View_Models
{
    public class MemberRegisterVm
    {
        [MaxLength(20),Required]
        public string UserName { get; set; }
        [MaxLength(50), Required]

        public string Fullname { get; set; }
        [MaxLength(100), Required,DataType(DataType.EmailAddress)]

        public string Email { get; set; }
        [MaxLength(20), Required,DataType(DataType.Password)]
        public string Password { get; set; }
        [MaxLength(20), Compare(nameof(Password)),DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
