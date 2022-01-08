# MVC Blog Demonstration Repository

This is a public repository created for documentation purposes and to demonstrate code used in the project for my personal blog.

Below you will find implementation details and code examples used throughout the project

## Blog Models

### This project makes use of several model classes which each pertain to a specific function of the blog

These components are outlines as follows:

### BlogUser:

```
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
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();


    }
```

Top-level model used to identity registered users which inherits from *Microsoft.AspNetCore.Identity.IdentityUser*

Contains information for the user's name, profile image, social media links, and referential relationships to the Blogs, Posts and/or Comments the user owns.

### Blog:

```
    /// <summary>
    /// Blog model use for database migrations
    /// </summary>
    public class Blog
    {
        public int Id { get; set; } //PK
        public string BlogUserId { get; set; } //FK
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
```

Primary model used to contain all information pertaining to an individual blog; including a name, description, creation and update dates, and blog image data.

Also contains referential properties to the owner(BlogUser) and Posts.

### Post:

```
using Microsoft.AspNetCore.Http;
using MVCBlog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCBlog.Models
{
    /// <summary>
    /// Post model used for database migrations
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
```

Model resposible for Post related information. This includes the Blog it belongs to and its owner, a title, an abstract, the Post content, creation and update dates, readtime, readystatus, slug, views, likes, and image data.

**Slug** is used to create a unique identity for the post within the url. This functionality can be found as an implementation of the ISlugService interface at MVCBlog.Services.BasicSlugService. It is loaded via **dependancy injection** within the PostsController.

**Readystatus** is an enum containing the following values, this feature has functionality yet to be built into the application:

```
    public enum ReadyStatus
    {
        Incomplete,
        ProductionReady,
        PreviewReady
    }
```

The Post contains 2 parent relationships; to Blogs and BlogUsers, as well as 2 children relationships pointing to Comments and Tags, respectively. 

### Comment:

```
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
        public int Id { get; set; } //PK
        public int? ParentCommentId { get; set; } //PK - Self-Referencing Relationship

        public int PostId { get; set; } //FK

        public string BlogUserId { get; set; } //FK

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

        // Navigation/Relationship Properties

        //Parents

        public virtual Post Post { get; set; }
        public virtual BlogUser BlogUser { get; set; }
        public virtual BlogUser Moderator { get; set; }
        public virtual Comment ParentComment { get; set; }

        //Recursive

        public virtual HashSet<Comment> Replies { get; set; } = new HashSet<Comment>();
    }
}

```

The comment model is unique in that it encorporates a self-referential relationship - meaning it refers to entities of the same type. This is due to the fact that comments may be nested and thus a way to determine the parent and child level comments is required.

The comment model contains properties for the comment body, create, update, moderated and deleted dates, moderated body, moderationtype, and comment level.

**ModeratedBody** contains the content which is used to override a comment when a comment is moderated by a user with the Moderator user role. This functionality has yet to be fully implemented. 

**ModerationType** is a enum to be used as a flag defining the reason why a comment has been moderated. The implementation of this enum can be seen here:
```
    public enum ModerationType
    {
        [Description("Political Propoganda")]
        Political,
        [Description("Offensive Language")]
        Language,
        [Description("Drug References")]
        Drugs,
        [Description("Theatening Speech")]
        Theatening,
        [Description("Sexual Content")]
        Sexual,
        [Description("Religous Discussion")]
        Religous,
        [Description("Hate Speech")]
        HateSpeech,
        [Description("Targeting Shaming")]
        Shaming,
        [Description("Other Type of Moderation")]
        Other
    }
```

Comments contain parental relationships to their Post, BlogUser(creator) and Moderator(in the case of a modified comment), as well as the parent comment. Comments also contain recursive relationship to all comments which are a within its reply chain defined as **Replies**.
