using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Areas.Manage.ViewModels
{
    public class AdminLoginVm
    {
        [MaxLength(20)]
        public string UserName { get; set; }
        [MaxLength(20),Required,DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
