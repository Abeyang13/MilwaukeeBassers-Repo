using FishingProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FishingProject.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult TournamentTeamAmountChart()
        {
            var tournaments = db.Tournaments.ToList();
            var allTeams = new List<ReportView>();
            foreach (Tournament tournament in tournaments)
            {
                var teams = db.TournamentTeams.Include(t => t.Team).Where(t => t.TournamentId == tournament.TournamentId && t.Tournament.TournamentDate < DateTime.Now);
                allTeams.Add(new ReportView
                {
                    DimensionOne = tournament.TournamentName,
                    Quantity = teams.Count()
                });
            }
            return View(allTeams);
        }

        public ActionResult AverageTeamWeightPerTeamChart()
        {
            List<Tournament> tournaments = db.Tournaments.ToList();
            var averageWeight = new List<ReportView>();
            
            foreach (Tournament tournament in tournaments)
            {
                List<double> weights = db.TournamentTeams.Where(t => t.TournamentId == tournament.TournamentId && t.Tournament.TournamentDate < DateTime.Now).Select(t => t.TotalWeight).ToList();
                double average = 0;
                foreach (double weight in weights)
                {
                    average += weight;
                }
                double overallAverage = average / weights.Count();
                averageWeight.Add(new ReportView
                {
                    DimensionOne = tournament.TournamentName,
                    Quantity = Math.Round(overallAverage * 100) / 100
                });
            }
            return View(averageWeight);
        }

        public ActionResult AverageBigBassPerTournament()
        {
            List<Tournament> tournaments = db.Tournaments.ToList();
            var averageBigBass = new List<ReportView>();

            foreach (Tournament tournament in tournaments)
            {
                List<double> bigBass = db.TournamentTeams.Where(t => t.TournamentId == tournament.TournamentId && t.Tournament.TournamentDate < DateTime.Now).Select(t => t.BigBass).ToList();
                double average = 0;
                foreach (double big in bigBass)
                {
                    average += big;
                }
                double overallBigBassAverage = average / bigBass.Count();
                averageBigBass.Add(new ReportView
                {
                    DimensionOne = tournament.TournamentName,
                    Quantity = Math.Round(overallBigBassAverage * 100) / 100
                });
            }
            return View(averageBigBass);
        }

        [HttpGet]
        public ViewResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactFormModel model, EmailAddress information)
        {
            if (ModelState.IsValid)
            {
                MailKitEmailService emailService = new MailKitEmailService(new EmailServerConfiguration());
                EmailMessage msgToSend = new EmailMessage
                {
                    FromAddresses = new List<ContactFormModel> { model },
                    ToAddresses = new List<EmailAddress> { information },
                    Content = $"Message From {model.Name} \n" +
                    $"Email: {model.Email} \n" + $"Message: {model.Message}",
                    Subject = "Contact Form"
                };

                emailService.Send(msgToSend);
                return RedirectToAction("Index");
            }
            else
            {
                return Contact();
            }
        }
    }
}