using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public class ReportView
    {
        [Key]
        public int ReportViewId { get; set; }
        public string DimensionOne { get; set; }
        public int Quantity { get; set; }
    }
}