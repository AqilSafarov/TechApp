using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Areas.Manage.ViewModels
{
    public class CategoryCreateVm
    {
        public int Id { get; set; }
        [Range(0,int.MaxValue)]
        public int Order { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }

    }
}
