using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CLS_SLE.Models
{
    public class AssessmentCategoriesController : Controller
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();

        // GET: AssessmentCategories
        public ActionResult Index()
        {
            return View(db.AssessmentCategories.ToList());
        }

        // GET: AssessmentCategories/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssessmentCategory assessmentCategory = db.AssessmentCategories.Find(id);
            if (assessmentCategory == null)
            {
                return HttpNotFound();
            }
            return View(assessmentCategory);
        }

        // GET: AssessmentCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AssessmentCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryCode,Name,Description,IsActive,CreatedDateTime,CreatedByLoginID,ModifiedDateTime,ModifiedByLoginID")] AssessmentCategory assessmentCategory)
        {
            if (ModelState.IsValid)
            {
                db.AssessmentCategories.Add(assessmentCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(assessmentCategory);
        }

        // GET: AssessmentCategories/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssessmentCategory assessmentCategory = db.AssessmentCategories.Find(id);
            if (assessmentCategory == null)
            {
                return HttpNotFound();
            }
            return View(assessmentCategory);
        }

        // POST: AssessmentCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryCode,Name,Description,IsActive,CreatedDateTime,CreatedByLoginID,ModifiedDateTime,ModifiedByLoginID")] AssessmentCategory assessmentCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assessmentCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(assessmentCategory);
        }

        // GET: AssessmentCategories/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssessmentCategory assessmentCategory = db.AssessmentCategories.Find(id);
            if (assessmentCategory == null)
            {
                return HttpNotFound();
            }
            return View(assessmentCategory);
        }

        // POST: AssessmentCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AssessmentCategory assessmentCategory = db.AssessmentCategories.Find(id);
            db.AssessmentCategories.Remove(assessmentCategory);
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
