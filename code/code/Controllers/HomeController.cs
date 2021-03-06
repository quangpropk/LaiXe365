﻿using code.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc;
using CaptchaMvc.Attributes;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc.Models;
namespace code.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        DBEntities db = new DBEntities();
        //DataContext db = new DataContext();
        public ActionResult Index()
        {
            
            var pages = db.Pages.Where(p=>p.IsHomePage==true).ToList();
            if(pages.Count > 0)
            {
                var page = pages.First();
                ViewBag.Content = page.Content;
            }
            
            return View();
        }
        public ActionResult PageByTitle(string title)
        {
            //string[] aliasArray = title.Split((".").ToCharArray());
            //if(aliasArray.Count() > 1)
            //{
            //    string alias = aliasArray[0];
                var pages = db.Pages.Where(p => p.Alias.Equals(title)).ToList();
                if (pages.Count > 0)
                {
                    var page = pages.First();
                    ViewBag.Content = page.Content;
                }
            //}
           
            return View();
        }
        public ActionResult page(int id)
        {
            var page = db.Pages.SingleOrDefault(p => p.ID == id);
            if (page == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = page.Title;
            ViewBag.Content = page.Content;
            return View();
        }
        [ChildActionOnly]
        public ActionResult header()
        {
            string headerFilePath = Server.MapPath("~/Files/header.txt");
            ViewBag.Text = Utilities.File.ReadFile(headerFilePath);
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult marquee()
        {
            string marqueeFilePath = Server.MapPath("~/Files/marquee.txt");
            ViewBag.Text = Utilities.File.ReadFile(marqueeFilePath);
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult menu()
        {
            return PartialView(db.Pages.OrderBy(p=>p.PageOrder).ToList());
        }

        [ChildActionOnly]
        public ActionResult moduleContact()
        {
            return PartialView(db.Contacts.ToList());
        }

        [ChildActionOnly]
        public ActionResult modulePost()
        {
            return PartialView(db.Posts.Where(p=>p.Type==1).Take(10).ToList());
        }

        [ChildActionOnly]
        public ActionResult slider()
        {
            try
            {
                string slideImagePath = Server.MapPath("~/Content/images/slider");
                DirectoryInfo di = new DirectoryInfo(slideImagePath);
                FileInfo[] files = di.GetFiles();
                List<string> imgNames = new List<string>();
                foreach (FileInfo file in files)
                {
                    imgNames.Add(file.Name);
                }
                ViewBag.FileNames = imgNames;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult comment()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult comment(Post post, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (this.IsCaptchaValid("Captcha is not valid"))
                    {
                        string imageName = "";
                        if (file != null && file.ContentLength > 0)
                        {
                            int size = file.ContentLength;
                            if (size >= 300000)
                            {
                                ViewBag.CaptchaError = "Kích thước file quá lớn";
                                return PartialView();
                            }
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/Content/images/baiviet"), fileName);
                            file.SaveAs(path);
                            imageName = fileName.ToString();
                        }
                        post.Alias = Utilities.EditString.BoDauTrenChuoi(post.Title);
                        post.Image = imageName;
                        post.CreatedDate = System.DateTime.Now;
                        post.PostView = 1;
                        post.Type = 2;
                        db.Posts.Add(post);
                        db.SaveChanges();
                        
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.CaptchaError = "Mã xác nhận không đúng";
                    }
                }
            }
            catch
            {
                ViewBag.CaptchaError = "Mã xác nhận không đúng";
            }
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult ShowComment()
        {
            return PartialView(db.Posts.Where(p=>p.Type==2).OrderByDescending(p=>p.ID).Take(4).ToList());
        }

        [ChildActionOnly]
        public ActionResult footer()
        {
            string footerFilePath = Server.MapPath("~/Files/footer.txt");
            ViewBag.Text = Utilities.File.ReadFile(footerFilePath);
            return PartialView();
        }
    }
}
