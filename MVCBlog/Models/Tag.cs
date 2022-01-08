using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Models
{
    /// <summary>
    /// Tag model use for database migrations
    /// </summary>
    public class Tag
    {
        public int Id { get; set; } //PK
        public int PostId { get; set; } //FK
        public string BlogUserId { get; set; } //FK

        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        public string Text { get; set; }

        //Navigation/Relationship Properties

        //Parents
        public virtual Post Post { get; set; } //Navigate to Post attached to Tag
        public virtual BlogUser BlogUser { get; set; }
    }
}
