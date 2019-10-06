using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FishingProject.Models
{
    public interface IEmailService
    {
        void Send(EmailMessage message);
    }
}