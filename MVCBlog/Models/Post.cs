using Microsoft.AspNetCore.Http;
using MVCBlog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCBlog.Models
{
    /// <summary>
    /// Post model use for database migrations
    /// </summary>
    public class Post
    {
        public int Id { get; set; } //PK
        [Display(Name ="Blog Name")]
        public int BlogId { get; set; } //FK
        public string BlogUserId { get; set; } //FK

        [Required]
        [StringLength(75, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        public string Abstract { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name="Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name="Updated Date")]
        public DateTime? Updated { get; set; }

        [Display(Name = "Read Time")]
        public int ReadTime { get; set; } 

        public ReadyStatus ReadyStatus { get; set; } //Enum

        public string Slug { get; set; } //Used for SEO/Custom Routing

        [Display(Name = "Views")]
        public int PageViews { get; set; } 

        [Display(Name = "Liked")]
        public int PostLikes { get; set; }

        [Display(Name="Post Image")]
        public byte[] ImageData { get; set; }

        [Display(Name="Image Type")]
        public string ContentType { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        //Navigation/Relationship Properties

        //Parents
        public virtual Blog Blog { get; set; }
        public virtual BlogUser BlogUser { get; set; }

        //Children
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
    }
}
