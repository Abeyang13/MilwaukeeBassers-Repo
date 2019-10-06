using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public class EmailMessage
    {
        public List<EmailAddress> ToAddresses { get; set; } = new List<EmailAddress>();
        public List<ContactFormModel> FromAddresses { get; set; } = new List<ContactFormModel>();
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}