using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Controllers
{
    public class TopicController:Controller
    {
        private readonly ForumDbContext context;
        public TopicController(ForumDbContext db)
        {
            this.context = db;
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var topic = this.context
                .Topics          
                .Include(t=>t.Author)
                .Include(t => t.Category)
                .Include(t => t.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefault(t => t.Id == id);

            if (topic == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(topic);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var categoryNames = context
                .Categories
                .Select(c => c.Name)
                .ToList();

            ViewData["CategoryNames"] = categoryNames;
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(string categoryName,Topic topic)
        {
            
            if (ModelState.IsValid)
            {               
                topic.CreatedDate = DateTime.Now;
                topic.LastUpdatedDate = DateTime.Now;   
                
                var authorId = context.Users
                    .Where(u => u.UserName == User.Identity.Name)
                    .FirstOrDefault()
                    .Id;

                topic.AuthorId = authorId;
                if (!context.Categories.Any(c => c.Name == categoryName))
                {
                    return View(topic);
                }
                var categoryId = context.Categories.FirstOrDefault(c => c.Name == categoryName).Id;
                topic.CategoryId = categoryId;

                context.Topics.Add(topic);
                context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }            
            return View(topic);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var topic = this.context.Topics.Include(t => t.Author).FirstOrDefault(t => t.Id == id);
            if (topic == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (topic.Author.UserName == User.Identity.Name)
            {
                return View(topic);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var topic = context.Topics.Include(t => t.Author).FirstOrDefault(t=>t.Id==id);
            if (topic==null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (topic.Author.UserName==User.Identity.Name)
            {
                context.Topics.Remove(topic);
                context.SaveChanges();
            }         

                return RedirectToAction("Index", "Home");           
            
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var topic = context
                .Topics                
                .Include(t => t.Author)
                .Include(t => t.Category)
                .FirstOrDefault(t=>t.Id==id);

            if (topic == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (topic.Author.UserName != User.Identity.Name)
            {
                return RedirectToAction("Index", "Home");
            }
            var categoryNames = context.Categories.Select(c => c.Name).ToList();
            ViewData["CategoryNames"] = categoryNames;
            return View(topic);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(string categoryName,Topic topic)
        {
            if (ModelState.IsValid)
            {
               var topicToEdit= context
                    .Topics
                    .Include(t => t.Author)
                    .FirstOrDefault(t=>t.Id==topic.Id);

                if (topicToEdit == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                topicToEdit.Title = topic.Title;
                topicToEdit.Description = topic.Description;
                
                topicToEdit.LastUpdatedDate = DateTime.Now;
              
                var categoryId = context
                    .Categories
                    .FirstOrDefault(c => c.Name == categoryName)
                    .Id;

                topicToEdit.CategoryId = categoryId;
                context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(topic);
        }
    }
}
