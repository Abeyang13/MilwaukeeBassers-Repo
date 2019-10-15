using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public bool PendingOrder { get; set; }

        [ForeignKey("Participant")]
        public int ParticipantId { get; set; }
        public Participant Participant { get; set; }
    }
}