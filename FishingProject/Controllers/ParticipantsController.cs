using FishingProject.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
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

        //GET: Participants/CreateTeam
        public ActionResult RegisterTeam()
        {
            Team team = new Team();
            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterTeam([Bind(Include = "TeamId,TeamName")] Team team)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity.GetUserId();
                Participant participant = db.Participants.Where(p => p.ApplicationId == currentUserId).Single();
                db.Teams.Add(team);
                db.SaveChanges();
                //TournamentTeam tournamentTeam = new TournamentTeam();
                //tournamentTeam.TournamentId = tournament.TournamentId;
                //tournamentTeam.TeamId = team.TeamId;
                //db.TournamentTeams.Add(tournamentTeam);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(team);
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