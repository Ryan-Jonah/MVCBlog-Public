# MVC Blog Demonstration Repository

This is a public repository created for documentation purposes and to demonstrate code used in the project for my personal blog.

Below you will find implementation details and code examples used throughout the project

##### Table of Contents  
1. [Overview](#overview)  
2. [Models](#models)  
    i. [BlogUser](#blogUserModel)  
    ii. [Blog](#blogModel)  
    iii. [Post](#postModel)  
    iv. [Comment](#commentModel)  
    v. [Tag](#tagModel)  
3. [Services](#services)  

<a name="overview"/> 

# Overview

### The project follows a standard blog layout which consists of the following pages:

| **Page** | **Function** |
| ---  |    ---   |
| Home Page | Entry point to the website. Contains a carousel linking to the top 4 most recent posts on the site with an list section to further expand on the posts shown. Here we are also able to see the header and browse by tag sections, which are present on all pages through the use of our \_layout.cshtml file.|
| About Page | Simple page containing information pertaining to myself and the project. |
| Blogs Page | Page which displays all user created blogs. Functionality is yet to be implemented for user created blogs.|
| Contact Page |A page used to send information directly to the site owner's email. This page makes use of the EmailService to handle incoming messages. |
| Register Page | Used to register a new user. This page was created in scaffolding.|
| Login Page | Used to login. This page was created during scaffolding.|
| Search Page | A page which shows results when using the search function or when selecting a tag. This page is defined within the PostsController, and using the BlogSearchService to curate a list of results.|
| Post Page |Page which contains the most functionality and created whenever a new post is added. Displays all information related to the post including it's content and the user who posted it. This page may be thought of as the comment page as well since all commments associated with the post are displayed here. Comments do not contain a page implementation on their own.|

<a name="models"/> 

## Blog Models

### This project makes use of several model classes which each pertain to a specific function of the blog

These components are outlined as follows:

<a name="blogUserModel"/> 

### BlogUser Model:

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

<a name="blogModel"/> 

### Blog Model:

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
```

Primary model used to contain all information pertaining to an individual blog; including a name, description, creation and update dates, and blog image data.

Also contains referential properties to the owner(BlogUser) and Posts.

<a name="postModel"/> 

### Post Model:

```
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

<a name="commentModel"/> 

### Comment Model: 

```
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

Comments contain parental relationships to their Post, BlogUser(creator) and Moderator(in the case of a modified comment), as well as the parent comment. Comments also contain a recursive relationship to all comments within its reply chain which is defined as **Replies**.

<a name="tagModel"/> 

### Tag Model

```
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
```
Simple model used to group and search for posts on the site. Contains a definition for the tag text as well as parent reationships to Post and BlogUser it belongs to.

<a name="services"/> 

## Blog Services

ASP.NET as a framework makes heavy use of a concept known as **dependancy injection(DI)** and **Inversion of Control(IoC)**.

These concepts allow components of the application to recieve other objects for which they are dependant on. Rather than instantiating an object as normal however, services are instead registered within **startup.cs** using an interface as well as an implementation of that interface. This is known as **Decoupling**, and allows different implementations of a given interface to be easily swapped and tested. These services can then be implemented using **Constructor Injection**. For example: 

```
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly ICommentService _commentService; 

        public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager, ICommentService commentService)
        {
            _context = context;
            _userManager = userManager;
            _commentService = commentService;
        }
```

Here we can see our database context, usermananger and custom comment service all definied as private fields within the CommentsController. 

Paying attention specifically to the comment service; we see we have defined a private readonly variable of \_commentService. This variable is the ICommentService, and is assigned a value through the commentService parameter within the constructor. Because this is the interface and not the implementation of the comment service, it is considered **Decoupled**, and can easily be swapped out in startup.cs if we ever decide to use a new implementation of the service. Pretty cool!

*Defining the comment service in startup.cs*

```
        public void ConfigureServices(IServiceCollection services)
        {
        
        //Other code...
        
            //Register Comment Service
            services.AddScoped<ICommentService, CommentService>();
```

#### Below I will outline the services that I have defined for this particular project.

