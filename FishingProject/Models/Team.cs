using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        [Display(Name = "Team Name")]
        public string TeamName { get; set; }
        public int NumberOfFishes { get; set; }
        public double TotalWeight { get; set; }
        public double BigBass { get; set; }

        [ForeignKey("Tournament")]
        public int TournamentId { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}