using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.View_Models
{
    public class MemberLoginVm
    {
        [MaxLength(100), Required, DataType(DataType.EmailAddress)]

        public string Email { get; set; }
        [MaxLength(20), Required, DataType(DataType.Password)]

        public string Password { get; set; }
    }
}
