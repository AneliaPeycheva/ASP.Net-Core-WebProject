﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Forum.Models;
using Forum.Data;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ForumDbContext context;
        public HomeController(ForumDbContext db)
        {
            this.context = db;
        }

        public IActionResult Index()
        {
            var topics = this.context
                .Topics
                .Include(t => t.Author)
                .Include(t=>t.Comments)
                .ThenInclude(c=>c.Author)
                .OrderByDescending(t => t.CreatedDate)
                .ThenByDescending(t => t.LastUpdatedDate)
                .ToList();

            var categories = this.context
                .Categories
                .Include(c => c.Author)
                .Include(c => c.Topics)
                .ThenInclude(t => t.Author)
                .ToList();

            ViewData["Categories"] = categories;
            return View(topics);
        }
    }
}
