using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public class Tournament
    {
        [Key]
        public int TournamentId { get; set; }
        [Display(Name = "Tournament Name")]
        public string TournamentName { get; set; }
        [Display(Name = "Tournament Date")]
        [DataType(DataType.Date)]
        public DateTime? TournamentDate { get; set; }
        public Location Location { get; set; }
        public List<Team> Teams { get; set; }
    }
}