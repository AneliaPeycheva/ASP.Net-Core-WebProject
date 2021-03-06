﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models
{
    public class Topic
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name ="Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name ="Last Updated Date")]
        public DateTime LastUpdatedDate { get; set; }
     
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();

        [NotMapped]
        [Display(Name = "Number Comments")]
        public int NumberComments => Comments.Count;

        public bool IsAuthor(string id)
        {
            return this.Author.UserName.Equals(id);
        }

    }
}
