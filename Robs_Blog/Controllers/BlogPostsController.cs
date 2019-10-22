using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Robs_Blog.Helpers;
using Robs_Blog.Models;
using PagedList;
using PagedList.Mvc;


namespace Robs_Blog.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index()
        {
            var blogPost = db.BlogPosts.ToList();
            return View(blogPost);
        }

        
        public ActionResult PubIndex()
        {
            var pubPosts = db.BlogPosts.Where(b => b.Published).ToList();
            return View("Index", pubPosts);
        }
        
        // GET: BlogPosts/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BlogPost blogPost = db.BlogPosts.Find(id);
        //    if (blogPost == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(blogPost);
        //}
        
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogpost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if (blogpost == null)
            {
                return HttpNotFound();
            }
            return View(blogpost);
        }


        [Authorize(Roles = "Admin")]  //only if logged in and admin

        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Abstract,BlogPostBody,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
           if (ModelState.IsValid)
            {     
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid Title");
                    return View(blogPost);
                }
                if(db.BlogPosts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    fileName = Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.Ticks + Path.GetExtension(fileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.ImagePath = "/Uploads/" + fileName;
                }
                blogPost.Slug = Slug;
                blogPost.Created = DateTime.Now;
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();                        

                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        public ActionResult Edit(string Slug)
        {
            if (string.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogpost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if(blogpost == null)
            {
                return HttpNotFound();
            }
            return View(blogpost);
        }


        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Slug,Abstract,BlogPostBody,MediaURL,Published,Created")] 
        BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid) 
            {
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid Title");
                }
                if (db.BlogPosts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                }
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.ImagePath = "/Uploads/" + fileName;
                }


                blogPost.Slug = Slug;                
                blogPost.Updated = DateTime.Now;
                db.Entry(blogPost).State = EntityState.Modified;  //Means that its being editing in the db 
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(string Slug)
        {
            if (string.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string Slug)
        {
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug );
            db.BlogPosts.Remove(blogPost);
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
