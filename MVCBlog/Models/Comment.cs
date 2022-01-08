using Microsoft.AspNetCore.Identity;
using MVCBlog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Models
{
    /// <summary>
    /// <para>Self-referencing relationship: A relationship in which the dependent and the principal entity types are the same.</para>
    /// <para>Definitions:</para>
    /// <para>Dependent entity: This is the entity that contains the foreign key properties. Sometimes referred to as the 'child' of the relationship.</para>
    /// <para>Principal entity: This is the entity that contains the primary/alternate key properties. Sometimes referred to as the 'parent' of the relationship.</para>
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Principal key: The properties that uniquely identify the principal entity. This may be the primary key or an alternate key.
        /// </summary>
        public int Id { get; set; } //PK

        /// <summary>
        /// <para>Principal key: The properties that uniquely identify the principal entity. This may be the primary key or an alternate key.</para>
        /// <para>Reference navigation property: A navigation property that holds a reference to a single related entity.</para>
        /// </summary>
        public int? ParentCommentId { get; set; } //PK - Self-Referencing Relationship

        //---//

        /// <summary>
        /// Foreign key: The properties in the dependent entity that are used to store the principal key values for the related entity.
        /// </summary>
        public int PostId { get; set; } //FK

        /// <summary>
        /// Foreign key: The properties in the dependent entity that are used to store the principal key values for the related entity.
        /// </summary>
        public string BlogUserId { get; set; } //FK

        /// <summary>
        /// Foreign key: The properties in the dependent entity that are used to store the principal key values for the related entity.
        /// </summary>
        public string ModeratorId { get; set; } //FK

        [Required]
        [StringLength(5000, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name="Comment")]
        public string Body { get; set; }

        [DataType(DataType.Date)]
        [Display(Name ="Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Moderated Date")]
        public DateTime? Moderated { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Deleted Date")]
        public DateTime? Deleted { get; set; }

        [StringLength(500, ErrorMessage = "The {0} must be between {2} and {1} characters.", MinimumLength = 2)]
        [Display(Name = "Moderated Comment")]
        public string ModeratedBody { get; set; }

        public ModerationType ModerationType { get; set; } //enum

        public int CommentLevel { get; set; }

        /*
         * Navigation/Relationship Properties
         * Created using the Foreign Key property names with "Id" removed
         * Used to obtain the record associated with the Foreign Key
        */

        //Parents

        /// <summary>
        /// <para>Navigation property: A property defined on the principal and/or dependent entity that references the related entity.</para>
        /// <para>Reference navigation property: A navigation property that holds a reference to a single related entity.</para>
        /// </summary>
        public virtual Post Post { get; set; }
        /// <summary>
        /// <para>Navigation property: A property defined on the principal and/or dependent entity that references the related entity.</para>
        /// <para>Reference navigation property: A navigation property that holds a reference to a single related entity.</para>
        /// </summary>
        public virtual BlogUser BlogUser { get; set; }
        /// <summary>
        /// <para>Navigation property: A property defined on the principal and/or dependent entity that references the related entity.</para>
        /// <para>Reference navigation property: A navigation property that holds a reference to a single related entity.</para>
        /// </summary>
        public virtual BlogUser Moderator { get; set; }
        /// <summary>
        /// <para>Navigation property: A property defined on the principal and/or dependent entity that references the related entity.</para>
        /// <para>Reference navigation property: A navigation property that holds a reference to a single related entity.</para>
        /// </summary>
        public virtual Comment ParentComment { get; set; }

        //Recursive

        /// <summary>
        /// <para>Navigation property: A property defined on the principal and/or dependent entity that references the related entity.</para>
        /// <para>Collection navigation property: A navigation property that contains references to many related entities.</para>
        /// </summary>
        public virtual HashSet<Comment> Replies { get; set; } = new HashSet<Comment>();
    }
}
