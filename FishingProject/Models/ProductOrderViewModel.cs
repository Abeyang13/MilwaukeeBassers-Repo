using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public class ProductOrderViewModel
    {
        public Product Product { get; set; }
        public List<Product> Products { get; set; }
    }
}