using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.Models;

namespace CLS_SLE.Controllers
{
    public class SectionEnrollmentsController : Controller
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();

        // GET: SectionEnrollments
        public ActionResult Index()
        {
            return View(db.SectionEnrollments.ToList());
        }

        // GET: SectionEnrollments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SectionEnrollment sectionEnrollment = db.SectionEnrollments.Find(id);
            if (sectionEnrollment == null)
            {
                return HttpNotFound();
            }
            return View(sectionEnrollment);
        }

        // GET: SectionEnrollments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SectionEnrollments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sectionID,EnrollmentID,StudentID,FirstName,LastName")] SectionEnrollment sectionEnrollment)
        {
            if (ModelState.IsValid)
            {
                db.SectionEnrollments.Add(sectionEnrollment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sectionEnrollment);
        }

        // GET: SectionEnrollments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SectionEnrollment sectionEnrollment = db.SectionEnrollments.Find(id);
            if (sectionEnrollment == null)
            {
                return HttpNotFound();
            }
            return View(sectionEnrollment);
        }

        // POST: SectionEnrollments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sectionID,EnrollmentID,StudentID,FirstName,LastName")] SectionEnrollment sectionEnrollment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sectionEnrollment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sectionEnrollment);
        }

        // GET: SectionEnrollments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SectionEnrollment sectionEnrollment = db.SectionEnrollments.Find(id);
            if (sectionEnrollment == null)
            {
                return HttpNotFound();
            }
            return View(sectionEnrollment);
        }

        // POST: SectionEnrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SectionEnrollment sectionEnrollment = db.SectionEnrollments.Find(id);
            db.SectionEnrollments.Remove(sectionEnrollment);
            db.SaveChanges();
            return RedirectToAction("Index");
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
