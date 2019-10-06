using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public class ProductOrderViewModel
    {
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}