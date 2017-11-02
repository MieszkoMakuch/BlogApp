using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;

namespace BlogApp.Controllers
{
    public class PublicationController : Controller
    {

        String blogUrl = "carblog";
        // GET: Publication
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET Publication/List
        public ActionResult List()
        {
            using (var database = new BlogDbContext())
            {
                var publications = database
                    .Publications
                    .Where(p => p.BlogId == blogUrl)
                    .Include(p => p.Author)
                    .ToList();

                return View(publications);
            }
        }

        //
        //GET Publication/ReadPost
        public ActionResult ReadPost(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("List");
            }

            using (var database = new BlogDbContext())
            {
                var publication = database
                    .Blogs
                    .FirstOrDefault(null)    
                    .Publications
                    .Where(p => p.Id == id)
                    .First();

                if (publication == null)
                {
                    return HttpNotFound();
                }

                return View(publication);
            }
        }

        //GET Publication/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //POST Publication/Create   
        [ValidateInput(false)]
        [HttpPost]
        [Authorize]
        public ActionResult Create(Publication publication)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    var authorId = database
                        .Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;
                    publication.AuthorId = authorId;
                    publication.DateTime = DateTime.Now;

                    database
                        .Blogs
                        .FirstOrDefault(null)    
                        .Publications.Add(publication);
                    database.SaveChanges();
                    return RedirectToAction("List");
                }
            }
            return View(publication);
        }

        //GET Publication/Create
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var publication = database
                    .Blogs
                    .FirstOrDefault(null)    
                    .Publications
                    .Where(p => p.Id == id)
                    .First();

                if (publication == null)
                {
                    return HttpNotFound();
                }
                return View(publication);
            }
        }

        //POST Publication/Edit   
        [ValidateInput(false)]
        [HttpPost]
        [Authorize]
        public ActionResult Edit(Publication publication)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    var publicationDb = database
                        .Blogs
                        .First()
                        .Publications
                        .First(p => p.Id == publication.Id);

                    publicationDb.Content = publication.Content;
                    publicationDb.Title = publication.Title;
                    publicationDb.Link = publication.Link;
                    database.SaveChanges();
                    return View();
                }
            }
            return View(publication);
        }

        //GET: Publication/Delete
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var publication = database
                    .Blogs
                    .FirstOrDefault(null)    
                    .Publications
                    .Where(p => p.Id == id)
                    .First();

                if (publication == null)
                {
                    return HttpNotFound();
                }
                return View(publication);
            }
        }


        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var publication = database
                    .Blogs
                    .First()
                    .Publications
                    .First(p => p.Id == id);

                if (publication == null)
                {
                    return HttpNotFound();
                }

                database
                    .Blogs
                    .FirstOrDefault(null)    
                    .Publications.Remove(publication);
                database.SaveChanges();
                return RedirectToAction("Index");
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment ([Bind(Include ="postID, name, message")] int postID, string name, string message)
        {
            var database = new BlogDbContext();
            Publication publication = database
                .Blogs
                .First()
                .Publications
                .First(p => p.Id == postID);

            Comment comment = new Comment();
            comment.postID = postID;
            comment.Email = User.Identity.Name;
            comment.CreatedDate = DateTime.Now;
            comment.Name = User.Identity.Name;
            comment.Body = message;
            comment.Publication = publication;

            database
                .Blogs
                .First()
                .Comments.Add(comment);
            database.SaveChanges();
            return RedirectToAction("ReadPost/" + publication.Id);
        }

        [Authorize]
        public ActionResult DeleteComment (int id)
        {
            using (var database = new BlogDbContext())
            {
                Comment comment = database
                    .Blogs
                    .First()
                    .Comments.First(c => c.ID == id);
                if(comment ==null)
                {
                    return HttpNotFound();
                }
                Publication publication = comment.Publication;
               
                database
                    .Blogs
                    .First()
                    .Comments.Remove(comment);
                database.SaveChanges();

                return RedirectToAction("ReadPost/" + publication.Id);
            }
        }
    }
}