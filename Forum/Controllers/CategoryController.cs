using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ForumDbContext context;
        public CategoryController(ForumDbContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var authorId = context
                    .Users
                    .Where(u => u.UserName == User.Identity.Name)
                    .FirstOrDefault()
                    .Id;
                
                category.AuthorId = authorId;
                context.Categories.Add(category);
                context.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            return View(category);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var category = context
                .Categories
                .Include(c=>c.Author)
                .Include(c=>c.Topics)
                .ThenInclude(t=>t.Author)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var allCategories = context
                .Categories
                .Include(c=>c.Topics)
                .ToList();

            ViewData["Categories"] = allCategories;
            return View(category);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var categoryToEdit = context
                .Categories
                .Include(c=>c.Author)
                .FirstOrDefault(c => c.Id == id);

            if (categoryToEdit == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (categoryToEdit.Author.UserName != User.Identity.Name)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(categoryToEdit);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var categoryToEdit = context
                    .Categories
                    .Include(c=>c.Author)
                    .FirstOrDefault(c => c.Id == category.Id);

                if (categoryToEdit == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (categoryToEdit.Author.UserName != User.Identity.Name)
                {
                    return RedirectToAction("Index", "Home");
                }

                categoryToEdit.Name = category.Name;               
                context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(category);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var categoryToDelete = context
                .Categories
                .Include(c=>c.Author)
                .FirstOrDefault(c => c.Id == id);

            if (categoryToDelete == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (categoryToDelete.Author.UserName != User.Identity.Name)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(categoryToDelete);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Delete(int id)
        {
           
            var categoryToDelete = context
                .Categories
                .Include(c=>c.Author)
                .FirstOrDefault(c => c.Id == id); 
            
            if (categoryToDelete == null)
            {
                return RedirectPermanent("/");
            }

            if (categoryToDelete.Author.UserName == User.Identity.Name)
            {               
                context.Categories.Remove(categoryToDelete);
                context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}