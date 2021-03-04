using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString =
            @"Data Source=.\sqlexpress;Initial Catalog=Blog;Integrated Security=true;";
        public IActionResult Index(int currentPage)
        {
            if(currentPage == 0)
            {
                currentPage = 1;
            }
            BlogDb db = new(_connectionString);
            BlogPostsViewModel vm = new()
            {
                Posts = db.GetPosts((currentPage-1) * 5),
                TotalPages = db.GetTotalPages(),
                CurrentPage = currentPage
            };
            return View(vm);
        }
        public IActionResult AddPost()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitPost(Post post)
        {
            BlogDb db = new(_connectionString);
            db.AddPost(post);
            return Redirect("/home/Index");
        }
        [HttpPost]
        public IActionResult SubmitComment(Comment comment, int postId)
        {
            BlogDb db = new(_connectionString);
            db.AddComment(comment, postId);
            Response.Cookies.Append("name", comment.Name);
            return Redirect("/home/Index");
        }
        public IActionResult ViewPost(int id)
        {
            BlogDb db = new(_connectionString);
            Post post = db.GetPost(id);
            post = db.AddCommentsForPost(post);
            ViewPostViewModel vm = new()
            {
                Post = post,
                Name = Request.Cookies["name"]
            };
            return View(vm);
        }
        public IActionResult MostRecent()
        {
            BlogDb db = new(_connectionString);
            int maxId = db.GetMostRecentPostId();
            if(maxId == -1)
            {
                return Redirect("/home/index");
            }
            return Redirect($"/home/viewpost?id={maxId}");
        }
    }
}
