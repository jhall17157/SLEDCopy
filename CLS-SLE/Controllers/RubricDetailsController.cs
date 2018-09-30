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
    public class RubricDetailsController : Controller
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();

        // GET: RubricDetails
        public ActionResult Index()
        {
            return View(db.RubricDetails.ToList());
        }

        // GET: RubricDetails/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RubricDetail rubricDetail = db.RubricDetails.Find(id);
            if (rubricDetail == null)
            {
                return HttpNotFound();
            }
            return View(rubricDetail);
        }

        // GET: RubricDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RubricDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AssessmentName,RubricID,RubricName,Description,OutcomeName,OutcomeSortOrder,CriteriaID,CriteriaName,ExampleText,CriteriaSortOrder")] RubricDetail rubricDetail)
        {
            if (ModelState.IsValid)
            {
                db.RubricDetails.Add(rubricDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rubricDetail);
        }

        // GET: RubricDetails/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RubricDetail rubricDetail = db.RubricDetails.Find(id);
            if (rubricDetail == null)
            {
                return HttpNotFound();
            }
            return View(rubricDetail);
        }

        // POST: RubricDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AssessmentName,RubricID,RubricName,Description,OutcomeName,OutcomeSortOrder,CriteriaID,CriteriaName,ExampleText,CriteriaSortOrder")] RubricDetail rubricDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rubricDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rubricDetail);
        }

        // GET: RubricDetails/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RubricDetail rubricDetail = db.RubricDetails.Find(id);
            if (rubricDetail == null)
            {
                return HttpNotFound();
            }
            return View(rubricDetail);
        }

        // POST: RubricDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            RubricDetail rubricDetail = db.RubricDetails.Find(id);
            db.RubricDetails.Remove(rubricDetail);
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
