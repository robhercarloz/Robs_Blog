using Robs_Blog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;


namespace Robs_Blog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        //Connection to database
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(int? page, string searchStr)
        {
            //Getting published blog post WHERE they're marked as published
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(blogList.ToPagedList(pageNumber, pageSize));
        }

        //added method-------------------------------------------------------------------------------------------------
        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) ||
                                     (p.BlogPostBody.Contains(searchStr) ||
                                      p.Comments.Any(c => c.CommentBody.Contains(searchStr) ||
                                                     c.Author.FirstName.Contains(searchStr) ||
                                                     c.Author.LastName.Contains(searchStr) ||
                                                     c.Author.DisplayName.Contains(searchStr) ||
                                                     c.Author.Email.Contains(searchStr))));
            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }
            return result.OrderByDescending(p => p.Created);
        }

        //------------------------------------------------------------------------------------------------------------------------
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            var publishedBlogPosts = db.BlogPosts.Where(b => b.Published).ToList();
            return View(publishedBlogPosts);
        }

        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";
            EmailModel model = new EmailModel();
            return View(model);
        }

        //New Action Created for email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var from = $"{model.FromEmail}<{ConfigurationManager.AppSettings["emailto"]}>";

                    var email = new MailMessage(from,
                                ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = model.subject,
                        Body = $"<strong>{model.FromName} has sent you the following message</strong> </hr> {model.Body}",
                        IsBodyHtml = true
                    };
                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);
                    return RedirectToAction("Index");


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }        
    }
}