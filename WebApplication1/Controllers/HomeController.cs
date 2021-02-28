using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebApplication1.Models;
namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private JobsDbEntities10 db = new JobsDbEntities10();
        public ActionResult Index1()
        {
            return View(db.JobDetails.ToList());
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Details1(int? JobId)
        {
            if (JobId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobDetails jobdetails = db.JobDetails.Find(JobId);
            if (jobdetails == null)
            {
                return HttpNotFound();
            }
            Session["JobId"] = JobId;
            return View(jobdetails);
        }
        public ActionResult ApplyJob1()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ApplyJob1(ApplyforJob applyjob, string message, HttpPostedFileBase uploadfilee)
        {
            if (HttpContext.Session["ProfileId"] != null && HttpContext.Session["ProfileId"].ToString() != "")
            { 
                var ProfileId = db.Profile.Select(c => c.ProfileId);
                var address= (from d in db.Profile where d.ProfileId == ProfileId.FirstOrDefault() select d.Address).FirstOrDefault();
                if (address != String.Empty)
                {
                    var JobId = (int)Session["JobId"];
                    var check = db.ApplyforJob.Where(a => a.JobId == JobId && a.ProfileId == ProfileId.FirstOrDefault()).ToList(); 
                    if (check.Count < 1)
                    {
                        if (uploadfilee != null)
                        {
                            applyjob.Message = message;
                            applyjob.ApplyDate = DateTime.Now;
                            string path = Path.Combine(Server.MapPath("~/images"), uploadfilee.FileName);
                            uploadfilee.SaveAs(path);
                            applyjob.uploadfile = uploadfilee.FileName;
                            applyjob.JobId = JobId;
                            applyjob.ProfileId = ProfileId.FirstOrDefault();
                            db.ApplyforJob.Add(applyjob);
                            db.SaveChanges();
                            ViewBag.result = "suceed to progess the job ";
                            return RedirectToAction("Index1");
                        }
                    }
                    else
                    {
                        ViewBag.Result = "You have already applied for the job";
                        return RedirectToAction("Index1");

                    }
                }
            }
            else
            {
                return RedirectToAction("login1", "Login");

            }
            return View(applyjob);
              
        }
        public ActionResult Download(string fileName)
        {
            string fullPath = Path.Combine(Server.MapPath("~/images"), fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public ActionResult GetJobsByUser1(HttpPostedFileBase CV,  int id = 0)
        {
            if (HttpContext.Session["ProfileId"] != null && HttpContext.Session["ProfileId"].ToString() != "")
            {
                var profileId = db.Profile.Select(c => c.ProfileId);
                var address = (from d in db.Profile where d.ProfileId == profileId.FirstOrDefault() select d.Address).FirstOrDefault();
                if (address != String.Empty)
                {
                    var Profileid = db.Profile.Select(c => c.ProfileId).FirstOrDefault();
                    var PRO = db.ApplyforJob.Where(a => a.ProfileId == Profileid);
                    if (PRO != null && Session["JobId"] != null)
                    {
                        var ProfileId = db.Profile.Select(c => c.ProfileId).FirstOrDefault();
                        var jobs = db.ApplyforJob.Where(a => a.ProfileId == ProfileId);
                        return View(jobs.ToList());
                    }
                    return View(PRO);
                }
            }
            return RedirectToAction("login1", "Login");
        }
        [HttpPost]
        public ActionResult DeleteJobByUser(int? ApplierId)
        {
            ApplyforJob applyforJob = db.ApplyforJob.Find(ApplierId);
            db.ApplyforJob.Remove(applyforJob);
            db.SaveChanges();
            return RedirectToAction("Index1");
        }

        public ActionResult GetJobsByPublisher1()
        {
            if (HttpContext.Session["ProfileId"] != null && HttpContext.Session["ProfileId"].ToString() != "")
            {
                var profileId = db.Profile.Select(c => c.ProfileId);
                var address = (from d in db.Profile where d.ProfileId == profileId.FirstOrDefault() select d.Address).FirstOrDefault();
                if (address != String.Empty)
                {
 
                    var ProfileId = db.Profile.Select(c => c.ProfileId).FirstOrDefault();
                    var jobs = from app in db.ApplyforJob
                               join job in db.JobDetails
                               on app.JobId equals job.JobId
                               where job.Profile.ProfileId == ProfileId
                               select app;
                    var grouped = from j in jobs
                                  group j by j.JobDetails.JobTitle
                                      into gr
                                      select new viewmodel
                                      {
                                          JobTitle = gr.Key,
                                          Items = gr
                                      };
                    return View(grouped.ToList());
                }
            }
            return RedirectToAction("login1", "Login");

        }
        [HttpPost]
        public ActionResult Search1(string SearchName)
        {
            var results = db.JobDetails.Where(a => a.JobTitle.Contains(SearchName) || SearchName == ""
               || a.JobType.Contains(SearchName) || SearchName == ""
               || a.CompanyName.Contains(SearchName) || SearchName == "").OrderBy(x => x.JobTitle).ToList();
            return View("Index1", results);
        }
        public ActionResult YouDontApply()
        {
            //ViewBag.Message = "Your application description page.";
            return View();
        }
        public ActionResult Contact1()
        {      
            return View();
        }
        [HttpPost]
        public ActionResult Contact1(RepliesSend contact)
        {
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                WebMail.EnableSsl = true;
                WebMail.UserName = "basant1998yasser@gmail.com";
                WebMail.Password = "facultyy";
                //Sender email address.  
                WebMail.From = "SenderGamilId@gmail.com";
                //Send email  
                WebMail.Send(to: contact.Email, subject: contact.Object, body: contact.Message, isBodyHtml: true);
                ViewBag.Status = "Email Sent Successfully.";
            }
            catch (Exception)
            {
                ViewBag.Status = "Problem while sending email, Please check details.";
            }
            return View();
        }
        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            } return sMacAddress;
        }
    }
}