using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Models
{
    /// <summary>
    /// Blog model use for database migrations
    /// </summary>
    public class Blog
    {
        public int Id { get; set; } //PK
        public string BlogUserId { get; set; } //FK

        [Required]
        //{0}: Name {1}: Max {2}: Min Data Annotations
        [StringLength(100, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)] 
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)] 
        public string Description { get; set; }

        [DataType(DataType.Date)] //Used in forms to omit time data
        [Display(Name="Created Date")] //Used in from labels
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name="Updated Date")] 
        public DateTime? Updated { get; set; }

        [Display(Name="Blog Image")]
        public byte[] ImageData { get; set; }

        [Display(Name="Image Type")]
        public string ContentType { get; set; }

        [NotMapped] //Does not map to the database
        //Used to extract information for ImageData + ContentType through HTTP Request
        public IFormFile Image { get; set; }

        //Navigation/Relationship Properties

        //Parents
        [Display(Name="Author")]
        public virtual BlogUser BlogUser { get; set; } //Parent of Blog, Post, Comment and Tag

        //Children
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>(); //Collection of Post children

    }
}

