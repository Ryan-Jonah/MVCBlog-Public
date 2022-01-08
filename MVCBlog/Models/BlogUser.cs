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
    /// IndentityUser class further defined through the use of inheritance
    /// 
    /// <para>Creates AspNetUsers Table</para>
    /// </summary>
    public class BlogUser : IdentityUser
    {
        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName
        { 
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        [Display(Name ="User Image")]
        public byte[] ImageData { get; set; }

        [Display(Name ="Image Type")]
        public string ContentType { get; set; }

        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }

        //Navigation/Relationship Properties

        //Children
        public virtual ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();

    }
}
