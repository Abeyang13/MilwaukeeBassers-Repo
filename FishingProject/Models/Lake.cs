﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public class Lake
    {
        [Key]
        public int LakeId { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
    }
}