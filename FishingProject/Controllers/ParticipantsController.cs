using FishingProject.Models;
using Microsoft.AspNet.Identity;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace FishingProject.Controllers
{
    public class ParticipantsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Participants
        public ActionResult Index()
        {
            var participantId = User.Identity.GetUserId();
            var participant = db.Participants.Where(p => p.ApplicationId == participantId).ToList();
            return View(participant);
        }

        // GET: Participants/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Participants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParticipantId,FirstName,LastName,TeamId,ApplicationId")] Participant participant)
        {

            if (ModelState.IsValid)
            {
                participant.ApplicationId = User.Identity.GetUserId();
                db.Participants.Add(participant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(participant);
        }

        // GET: Participants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }

        // GET: Participants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }

        // POST: Participants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ParticipantId,FirstName,LastName,TeamId,ApplicationId")] Participant participant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(participant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(participant);
        }

        // GET: Participants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = db.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }

        // POST: Participants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Participant participant = db.Participants.Find(id);
            db.Participants.Remove(participant);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Participants/CreateTeam when registering for a certain tournament
        public ActionResult RegisterTeam(int id)
        {
            TournamentTeamViewModels tournamentTeamView = new TournamentTeamViewModels();
            var tournament = db.Tournaments.FirstOrDefault(t => t.TournamentId == id);
            tournamentTeamView.Tournament = tournament;
            return View(tournamentTeamView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterTeam(TournamentTeamViewModels teamViewModels)
        {
                var currentUserId = User.Identity.GetUserId();
                Participant participant = db.Participants.Where(p => p.ApplicationId == currentUserId).Single();
                if(participant.TeamId == null)
                {
                    participant.TeamId = teamViewModels.Team.TeamId;
                }
                else
                {
                teamViewModels.Team.TeamId = participant.TeamId.Value;
                }
                db.Teams.Add(teamViewModels.Team);
                db.SaveChanges();

                TournamentTeam tournamentTeam = new TournamentTeam();
                tournamentTeam.TeamId = teamViewModels.Team.TeamId;
                tournamentTeam.TournamentId = teamViewModels.Tournament.TournamentId;
                db.TournamentTeams.Add(tournamentTeam);
                db.SaveChanges();
                return RedirectToAction("TournamentIndex", "Organizations");
        }

        [HttpPost]
        public ActionResult Charge(string stripeEmail, string stripeToken)
        {
            var currentCustomer = User.Identity.GetUserId();
            var user = db.Participants.Where(p => p.ApplicationId == currentCustomer).Single();
            var order = db.ProductOrders.Include(p => p.Product).Where(p => p.Order.ParticipantId == user.ParticipantId).ToList();
            decimal total = 0;
            foreach (var product in order)
            {
                total += product.Total;
            }

            var customers = new CustomerService();
            var charges = new ChargeService();
            StripeConfiguration.ApiKey = Keys.SecretKey;
            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var currentUserId = User.Identity.GetUserId().ToString();
            var participant = db.Participants.FirstOrDefault(p => p.ApplicationId == currentUserId);
            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = Convert.ToInt64(total * 100),
                Description = "Sample Charge",
                Currency = "usd",
                CustomerId = customer.Id
            });

            var completeOrder = db.Orders.Where(o => o.ParticipantId == user.ParticipantId && o.PendingOrder == true).Single();
            completeOrder.PendingOrder = false;
            db.SaveChanges();
            foreach (var product in order)
            {
                product.Paid = true;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult StripeIndex()
        {
            var stripePublishKey = Keys.PublishableKey;
            ViewBag.StripePublishKey = stripePublishKey;
            return View();
        }

        //ADD Order If Order Is Not Pending/Else ADD Product To Existing Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrder(Models.Product product)
        {
            var newCustomer = User.Identity.GetUserId();
            Participant participant = db.Participants.Where(p => p.ApplicationId == newCustomer).Single();
            Models.Order order = db.Orders.Where(o => o.ParticipantId == participant.ParticipantId && o.PendingOrder == true).FirstOrDefault();
            if (order == null)
            {
                var newOrder = new Models.Order();
                newOrder.ParticipantId = participant.ParticipantId;
                newOrder.PendingOrder = true;
                db.Orders.Add(newOrder);
                db.SaveChanges();

                ProductOrder productOrder = new ProductOrder();
                productOrder.OrderId = newOrder.OrderId;
                productOrder.ProductId = product.ProductId;
                productOrder.Quantity = product.Quantity;
                productOrder.Size = product.Size;
                productOrder.Total = product.Quantity * product.Price;
                db.ProductOrders.Add(productOrder);
                db.SaveChanges();
            }
            else
            {
                ProductOrder productOrder = new ProductOrder();
                productOrder.OrderId = order.OrderId;
                productOrder.ProductId = product.ProductId;
                productOrder.Quantity = product.Quantity;
                productOrder.Size = product.Size;
                productOrder.Total = product.Quantity * product.Price;
                db.ProductOrders.Add(productOrder);
                db.SaveChanges();
            }
            return RedirectToAction("ViewOrder");
        }
        //GET Order That Contains All Products For That Order
        public ActionResult ViewOrder()
        {
            var currentCustomer = User.Identity.GetUserId();
            var customer = db.Participants.Where(p => p.ApplicationId == currentCustomer).Single();
            var order = db.ProductOrders.Include(p => p.Product).Where(p => p.Order.ParticipantId == customer.ParticipantId && p.Paid == false).ToList();
            decimal total = 0;
            foreach (var product in order)
            {
                total += product.Total;
            }
            ViewBag.Total = total;
            return View(order);
        }

        //Get Product id
        public ActionResult EditOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductOrder productOrder = db.ProductOrders.Include(p => p.Product).Include(p => p.Order).Where(p => p.ProductOrderId == id).Single();
            if (productOrder == null)
            {
                return HttpNotFound();
            }
            return View(productOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder(ProductOrder productOrder)
        {
            var productId = db.ProductOrders.Include(p => p.Product).Include(p => p.Order).Where(p => p.ProductOrderId == productOrder.ProductOrderId).Single();
            productId.Quantity = productOrder.Quantity;
            productId.Size = productOrder.Product.Size;
            productId.Total = productOrder.Quantity * productId.Product.Price;
            db.Entry(productId).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewOrder");
        }

        public ActionResult DeleteProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductOrder productOrder = db.ProductOrders.Include(p => p.Product).Include(p => p.Order).Where(p => p.ProductOrderId == id).Single();
            if (productOrder == null)
            {
                return HttpNotFound();
            }
            return View(productOrder);
        }

        // POST: Delete Product from Order
        [HttpPost]
        public ActionResult DeleteProduct(ProductOrder productOrder)
        {
            var productOrderId = db.ProductOrders.Find(productOrder.ProductOrderId);
            db.ProductOrders.Remove(productOrderId);
            db.SaveChanges();
            return RedirectToAction("ViewOrder");
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