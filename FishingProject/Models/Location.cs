using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        [Display(Name = "Zip Code")]
        public int ZipCode { get; set; }
        public string Country { get; set; }
    }
}