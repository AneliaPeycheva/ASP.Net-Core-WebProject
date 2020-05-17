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
    public class CommentController : Controller
    {
        private readonly ForumDbContext context;

        public CommentController(ForumDbContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet]
        [Route("/Topic/Details/{id}/Comment/Create")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("/Topic/Details/{TopicId}/Comment/Create")]
        public IActionResult Create(Comment comment)
        {
            
            if (ModelState.IsValid)
            {
                var authorId = context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
                if (authorId == null)
                {
                    return Redirect($"/Topic/Details/{comment.TopicId}");
                }
                var newComment = new Comment
                {
                    Description = comment.Description,
                    CreatedDate = DateTime.Now,
                    LastUpdatedDate = DateTime.Now,
                    AuthorId = authorId,
                    TopicId=comment.TopicId
                };
                Topic topic = context
                    .Topics
                    .FirstOrDefault(t => t.Id == comment.TopicId);

                topic.LastUpdatedDate = DateTime.Now;
                context.Comments.Add(newComment);
                context.SaveChanges();
                return Redirect($"/Topic/Details/{comment.TopicId}");
            }
            
            return View(comment);
        }

        [Authorize]
        [HttpGet]
        [Route("/Topic/Details/{TopicId}/Comment/Edit/{id}")]
        public IActionResult Edit(int? TopicId,int? id)
        {          
            if (id == null)
            {
                return RedirectPermanent($"/Topic/Details/{TopicId}");
            }
            var comment = context.Comments.Find(id);
            if (comment == null)
            {
                return RedirectPermanent($"/Topic/Details/{TopicId}");
            }
            return View(comment);
        }

        [Authorize]
        [HttpPost]
        [Route("/Topic/Details/{TopicId}/Comment/Edit/{id}")]
        public IActionResult Edit(Comment comment)
        {         
            if (ModelState.IsValid)
            {               
                var commentToEdit = context
                    .Comments
                    .FirstOrDefault(c => c.CommentId == comment.CommentId);

                if (commentToEdit == null)
                {
                    return RedirectPermanent($"/Topic/Details/{comment.TopicId}");
                }
                commentToEdit.Description= comment.Description;
                commentToEdit.LastUpdatedDate = DateTime.Now;

                Topic topic = context
                    .Topics
                    .FirstOrDefault(t => t.Id == comment.TopicId);

                if (topic == null)
                {
                    return RedirectPermanent($"/Topic/Details/{comment.TopicId}");
                }

                topic.LastUpdatedDate = DateTime.Now;
               
                context.SaveChanges();
                return Redirect($"/Topic/Details/{comment.TopicId}");
            }
            return View(comment);
        }

        [Authorize]
        [HttpGet]
        [Route("Topic/Details/{TopicId}/Comment/Delete/{id}")]
        public IActionResult Delete(int? TopicId,int? id)
        {
            if (id == null)
            {
                return RedirectPermanent($"/Topic/Details/{TopicId}");
            }
            var comment = context
                .Comments
                .Include(c=>c.Author)
                .Include(c=>c.Topic)
                .ThenInclude(t=>t.Author)
                .FirstOrDefault(c=>c.CommentId==id);

            if (comment == null)
            { 
                return RedirectPermanent($"/Topic/Details/{TopicId}");
            }
            return View(comment);
        }

        [Authorize]
        [HttpPost]
        [Route("Topic/Details/{TopicId}/Comment/Delete/{id}")]
        public IActionResult Delete(int? id)
        {           
                var commentToDelete = context
                .Comments
                .FirstOrDefault(c => c.CommentId == id);

            if (commentToDelete!=null)
            {
                Topic topic = context
                    .Topics
                    .FirstOrDefault(t => t.Id == commentToDelete.TopicId);

                topic.LastUpdatedDate = DateTime.Now;
                context.Comments.Remove(commentToDelete);
                context.SaveChanges();               
            }
            return Redirect($"/Topic/Details/{commentToDelete.TopicId}");
        }
    }
}