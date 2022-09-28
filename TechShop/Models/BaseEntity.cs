using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime ModifideAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
