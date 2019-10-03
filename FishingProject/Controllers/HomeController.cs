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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TournamentTeamAmountChart()
        {
            var tournaments = db.Tournaments.ToList();
            List<int> AllTeams = new List<int>();
            var allTeams = new List<ReportView>();
            foreach (Tournament tournament in tournaments)
            {
                var teams = db.TournamentTeams.Include(t => t.Team).Where(t => t.TournamentId == tournament.TournamentId);
                AllTeams.Add(teams.Count());
                allTeams.Add(new ReportView
                {
                    DimensionOne = tournament.TournamentName,
                    Quantity = teams.Count()
                }); ;
            }
            return View(allTeams);
        }
    }
}