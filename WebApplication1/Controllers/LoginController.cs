using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class LoginController : Controller
    {
        private JobsDbEntities10 db = new JobsDbEntities10();
        [HttpGet]
        public ActionResult login1()
        {
            return View();
        }
        [HttpPost]
        public ActionResult login1(Profile pro, string txtname = "", string txtpass = "")
        {
            if (ModelState.IsValid)
            {
                var obj = db.Profile.Where(a => a.ProfileName == txtname && a.Password == txtpass).FirstOrDefault();
                if (obj != null)
                {
                    if (obj.RoleId == 2)
                    {
                        int? ProfileId = obj.ProfileId;
                        Session["ProfileId"] = ProfileId;
                        Session["UserName"] = obj.ProfileName.ToString();
                        Session["RoleName"] = obj.Roles.RoleName.ToString();
                    }
                    else
                    {
                        int? ProfileId = obj.ProfileId;
                        Session["ProfileId"] = obj.ProfileId.ToString();
                        Session["UserName"] = obj.ProfileName.ToString();
                        Session["RoleName"] = obj.Roles.RoleName.ToString();
                    }
                    int ui = obj.ProfileId;
                    (Session["UserID"]) = ui;
                    var address = (from d in db.Profile where d.ProfileId == ui select d.Address).FirstOrDefault();
                    if (address == null)
                    {
                        return RedirectToAction("Register1", "Home");
                    }                
                }
                else
                {
                    ViewBag.Result = "something wrong please enter your info correctly";
                    return View(pro);
                }
            }
            return RedirectToAction( "Index1","Home");
        }
        [HttpGet]
        public ActionResult Register1()
        {
            ViewBag.RoledId = new SelectList(db.Roles.Where(a => a.RoleName.Contains("User")).ToList(), "RoledId", "RoleName");
            ViewBag.RoledId = new SelectList(db.Roles.Where(a => a.RoleName.Contains("Pulisher")).ToList(), "RoledId", "RoleName");
            return View();
        }
        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register1(Profile pro, string txtname = "", string email = "", string txtpass = "", string confirmpass = "", string address = "")
        {
            var roles = db.Roles.ToList();
            var obj = db.Profile.Where(a => a.Email == email).FirstOrDefault();
            if (obj==null)
            {
                pro.ProfileName = txtname;
                pro.Password = txtpass;
                pro.Email = email;
                pro.Address = address;
                db.Profile.Add(pro);
                db.SaveChanges();
                ViewBag.RoledId = new SelectList(db.Roles.Where(a => a.RoleName.Contains("User")).ToList(), "RoledId", "RoleName", pro.RoleId);
                ViewBag.RoledId = new SelectList(db.Roles.Where(a => a.RoleName.Contains("Pulisher")).ToList(), "RoledId", "RoleName");
                return RedirectToAction("Index1", "Home");             
            }
            ViewBag.Message = "Please enter your information correctly";             
            return View(pro);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            Session.Clear();
            Session.Abandon();
            // Redirecting to Login page after deleting Session
            return RedirectToAction("Index1","Home");
        }       
        // GET: /Login/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profile.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", profile.RoleId);
            return View(profile);
        }

        // POST: /Login/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Profile profile, string txtname, string email, string txtpass, string confirmpass, string address)
        {

            if (ModelState.IsValid)
            {
                db.Entry(profile).State = EntityState.Modified;
                profile.ProfileName = txtname;
                profile.Email = email;
                profile.Password = txtpass;
                profile.Address = address;
                ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", profile.RoleId);
                db.SaveChanges();
                return RedirectToAction("Index1", "Home");
            }
            return View(profile);
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
