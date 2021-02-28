using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class JobDetailsController : Controller
    {
        private JobsDbEntities10 db = new JobsDbEntities10();
        // GET: /JobDetails/Create
        public ActionResult Create()
        {
            ViewBag.ProfileId = new SelectList(db.Profile, "ProfileId", "ProfileName");
            return View();
        }

        // POST: /JobDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(JobDetails jobdetails,HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    string path = Path.Combine(Server.MapPath("~/images"), upload.FileName);
                    upload.SaveAs(path);
                    jobdetails.JobImage = upload.FileName;
                }              
                db.JobDetails.Add(jobdetails);
                db.SaveChanges();
                return RedirectToAction("Index1", "Home");
            }
            ViewBag.ProfileId = new SelectList(db.Profile, "ProfileId", "ProfileName", jobdetails.ProfileId);
            return View(jobdetails);
        }
        public ActionResult EditJob(int? id)
        {
            var job = db.JobDetails.Where(a => a.JobId == id).FirstOrDefault();
            return View(job);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJob(JobDetails jobdetails, int id, string jobtitle, string companyname, string address, HttpPostedFileBase upload,
             string jobtype, string educationlevel, string languages, string vacancies, string salary,string jobrequirements)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jobdetails).State = EntityState.Modified;
                jobdetails = db.JobDetails.Where(a => a.JobId == id).FirstOrDefault();
                jobdetails.JobTitle = jobtitle;
                jobdetails.CompanyName = companyname;
                jobdetails.DateJob = DateTime.Now;
                jobdetails.Address = address;
                string path = Path.Combine(Server.MapPath("~/images"), upload.FileName);
                upload.SaveAs(path);
                jobdetails.JobImage = upload.FileName;
                jobdetails.JobType = jobtype;
                jobdetails.EducationLevel = educationlevel;
                jobdetails.Languages = languages;
                jobdetails.Salary= salary;
                jobdetails.Vacancies = vacancies;
                jobdetails.JobRequirements = jobrequirements;
                db.SaveChanges();
                return RedirectToAction("Index1");
            }
            else
                ViewBag.Result = "something wrong please enter your info correctly";
            return View(jobdetails);
        }
        public ActionResult DeleteJob(int? JobId)
        {
            JobDetails jobDetails = db.JobDetails.Find(JobId);
            db.JobDetails.Remove(jobDetails);
            db.SaveChanges();
            return RedirectToAction("Index1");
        }
    }
}
