using FishingProject.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
namespace FishingProject.Controllers
{
    public class OrganizationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Organizations
        public ActionResult Index()
        {
            var organizationId = User.Identity.GetUserId();
            var organization = db.Organizations.Where(o => o.ApplicationId == organizationId).ToList();
            return View(organization);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrganizationId,FirstName,LastName")] Organization organization)
        {
            if (ModelState.IsValid)
            {
                organization.ApplicationId = User.Identity.GetUserId();
                db.Organizations.Add(organization);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(organization);
        }

        // GET: Tournaments
        public ActionResult TournamentIndex()
        {
            var tournaments = db.Tournaments.ToList().OrderByDescending(t => t.TournamentDate);
            return View(tournaments);
        }
        public ActionResult CreateTournament()
        {
            Tournament tournament = new Tournament();
            ViewBag.Lakes = new SelectList(db.Lakes.ToList(), "LakeId", "Name");
            return View(tournament); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTournament([Bind(Include = "TournamentId,TournamentName,TournamentDate,LakeId,OrganizationId")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity.GetUserId();
                Organization organization = db.Organizations.Where(o => o.ApplicationId == currentUserId).Single();
                tournament.OrganizationId = organization.OrganizationId;
                db.Tournaments.Add(tournament);
                db.SaveChanges();
                return RedirectToAction("TournamentIndex");
            }
            return View(tournament);
        }

        //Get: Details of Tournament
        public ActionResult TournamentDetails(int? id)
        {
            var tournament = db.Tournaments.Include(t => t.Lake.Address).FirstOrDefault(t => t.TournamentId == id);
            return View(tournament);
        }

        // GET: Tournament/Edit/5
        public ActionResult EditTournamentTeam(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TournamentTeam tournamentTeam = db.TournamentTeams.Find(id);
            if (tournamentTeam == null)
            {
                return HttpNotFound();
            }
            return View(tournamentTeam);
        }

        // POST: Tournament/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTournamentTeam(TournamentTeam tournamentTeam)
        {
            var tournamentTeamId = db.TournamentTeams.Include(t => t.Team).Include(t => t.Tournament).Where(t => t.TournamentTeamId == tournamentTeam.TournamentTeamId).Single();
            tournamentTeamId.TotalWeight = tournamentTeam.TotalWeight;
            tournamentTeamId.BigBass = tournamentTeam.BigBass;
            tournamentTeamId.NumberOfFishes = tournamentTeam.NumberOfFishes;
            db.Entry(tournamentTeamId).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("TournamentTable", new { id = tournamentTeamId.TournamentId });
        }

        //GET: List of tournament teams
        public ActionResult TournamentTable(int id)
        {
            var tournamentId = db.TournamentTeams.OrderByDescending(t => t.TotalWeight).Include(t => t.Team).Where(t => t.TournamentId == id);
            return View(tournamentId);
        }
        
        // GET: Teams/Delete
        public ActionResult DeleteTeam(int? id)
     {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TournamentTeam tournamentTeam = db.TournamentTeams.Include(t => t.Team).Include(t => t.Tournament).Where(t => t.TournamentTeamId == id).FirstOrDefault();
            if (tournamentTeam == null)
            {
                return HttpNotFound();
            }
            return View(tournamentTeam);
        }

        // POST: Teams/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTeam(TournamentTeam tournamentTeam)
        {
            var tournamentTeamId = db.TournamentTeams.Find(tournamentTeam.TournamentTeamId);
            db.TournamentTeams.Remove(tournamentTeamId);
            db.SaveChanges();
            return RedirectToAction("TournamentTable", new { id = tournamentTeamId.TournamentId });
        }

        public string ConvertAddressToGoogleFormat(Address address)
        {
            string googleFormatAddress = address.StreetAddress + "," + address.City + "," + address.State + "," + address.ZipCode + "," + address.Country;
            return googleFormatAddress;
        }

        public GeoCode GeoLocate(string address)
        {
            var key = Keys.GoogleGeoCodeAPIKey;
            var requestUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={key}";
            var result = new WebClient().DownloadString(requestUrl);
            GeoCode geocode = JsonConvert.DeserializeObject<GeoCode>(result);
            return geocode;
        }

        public ActionResult CreateLake()
        {
            Lake lake = new Lake();
            return View(lake);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLake([Bind(Include = "LakeId,Name,Address")] Lake lake)
        {
            if (ModelState.IsValid)
            {
                Address address = new Address();
                address = lake.Address;
                address.Country = "USA";
                string addressToConvert = ConvertAddressToGoogleFormat(address);
                var geoLocate = GeoLocate(addressToConvert);
                address.Longitude = geoLocate.results[0].geometry.location.lng;
                address.Latitude = geoLocate.results[0].geometry.location.lat;
                db.Lakes.Add(lake);
                db.SaveChanges();
                return RedirectToAction("TournamentIndex");
            }
            return View(lake);
        }

        public ActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct([Bind(Include = "Name,Price")] Product product, HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        var photo = new File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.Picture,
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            photo.Content = reader.ReadBytes(upload.ContentLength);
                        }
                        product.Files = new List<File> { photo };
                    }
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Merchandise");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again.");
            }
            return View(product);
        }

        //GET List Of All Products
        public ActionResult Merchandise()
        {
            ProductOrderViewModel productOrderViewModel = new ProductOrderViewModel();
            productOrderViewModel.Products = db.Products.Include(p => p.Files).ToList();
            return View(productOrderViewModel);
        }

        //Get All Orders
        public ActionResult ViewAllOrders()
        {
            var orders = db.ProductOrders.Include(p => p.Product).Where(p => p.Paid == true).ToList();
            return View(orders);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}