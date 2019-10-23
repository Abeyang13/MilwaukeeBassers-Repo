using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public class TournamentTeam
    {
        [Key]
        public int TournamentTeamId { get; set; }

        [ForeignKey("Team")]
        public int TeamId { get; set; }
        public Team Team { get; set; }

        [ForeignKey("Tournament")]
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }

        [Display(Name = "Big Bass")]
        public double BigBass { get; set; }
        [Display(Name = "# Of Fish")]
        public int NumberOfFishes { get; set; }
        [Display(Name = "Total Weight")]
        public double TotalWeight { get; set; }
    }
}