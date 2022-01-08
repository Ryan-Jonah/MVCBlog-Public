using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.ViewModels
{
    public class ContactMe
    {
        [Required]
        [StringLength(80, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        [StringLength(80, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        public string Subject { get; set; }
        [Required]
        [StringLength(1000, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        public string Message { get; set; }
    }
}
