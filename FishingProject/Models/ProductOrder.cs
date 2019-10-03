using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public class ProductOrder
    {
        [Key]
        public int TournamentTeamId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product{ get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}