using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCBlog.Data;
using MVCBlog.Models;
using MVCBlog.Services;
using X.PagedList;
using MVCBlog.Enums;

namespace MVCBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly BlogSearchService _blogSearchService;
        private readonly ICommentService _commentService;
        private readonly IHtmlHelper _htmlHelper;
        
        public PostsController(
            ApplicationDbContext context,
            ISlugService slugService,
            IImageService imageService,
            UserManager<BlogUser> userManager,
            BlogSearchService blogSearchService, 
            ICommentService commentService,
            IHtmlHelper htmlHelper)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
            _blogSearchService = blogSearchService;
            _commentService = commentService;
            _htmlHelper = htmlHelper;
        }

        //POST: Search
        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {
            ViewData["searchTerm"] = searchTerm;

            var pageNumber = page ?? 1;
            var pageSize = 12;

            var posts = _blogSearchService.Search(searchTerm).Include(p => p.BlogUser);

            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser)//Allows specifying a different dataset 
                .Include(p => p.Comments)
                .ThenInclude(c => c.Replies)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null)
            {
                return NotFound();
            }

            //Previous/Next navigation ordered by posted date
            List<Post> orderedPosts = await _context.Posts.OrderBy(p => p.Created).ToListAsync();

            int iteration = 0;
            foreach (var item in orderedPosts)
            {
                //Determine the current post within the ordered list
                if (item.Id == post.Id)
                {
                    //Next will not apply on last item
                    if (iteration < (orderedPosts.Count - 1))
                    {
                        ViewData["nextPost"] = orderedPosts[iteration + 1];
                    }
                    //Previous will not apply on first item
                    if (iteration > 0)
                    {
                        ViewData["prevPost"] = orderedPosts[iteration - 1];
                    }
                }
                iteration++;
            }

            post.PageViews++;
            await _context.SaveChangesAsync();

            //Count comments
            ViewData["commentCount"] = post.Comments.Count.ToString();

            //Parent Comments
            HashSet<Comment> parentComments = _commentService.FilterParentComments(_context.Comments.ToHashSet());

            //Ordered Comments
            HashSet<Comment> orderedComments = new();

            //Assigning Ordered Comments
            foreach (var comment in parentComments)
            {
                orderedComments.Add(comment);

                foreach (var reply in await _commentService.GetCommentRepliesAsync(comment))
                {
                    orderedComments.Add(reply);
                }
            }

            //Ordered Comments ViewData
            ViewData["orderedComments"] = orderedComments;

            return View(post);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            //Displays the post name and sends back the post Id by using viewbag in the view
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogUser,BlogId,Title,Abstract,Content,ReadTime,ReadyStatus,Image")] Post post, List<string> tagValues)
        {
            //Saves posted Post date to the database if data is valid
            if (ModelState.IsValid)
            {
                //Adding timestamp
                post.Created = DateTime.Now;

                //Convert title to url slug
                var slug = _slugService.UrlFriendly(post.Title);

                //Assigns User Id to the currently logged in user
                var authorId = _userManager.GetUserId(User);
                post.BlogUserId = authorId;

                //Encode Image Data
                post.ImageData = await _imageService.EncodeImageAsync(post.Image);
                post.ContentType = _imageService.ContentType(post.Image);

                if (!_slugService.isUnique(slug))
                {
                    //Add a model state error and return user back to the create view
                    ModelState.AddModelError("Title", "The title you provided cannot be used as it results in a name collision.");

                    //Returns what the user already entered
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }

                //Create variable to detect if an error has occured
                var validationError = false;

                //Detect empty slugs
                if (string.IsNullOrEmpty(slug))
                {
                    ModelState.AddModelError("", "The title can not be blank.");
                    validationError = true;
                }

                //Detect incoming duplicate slugs
                else if (!_slugService.isUnique(slug))
                {
                    ModelState.AddModelError("Title", "The title you provided could not be used as it results in a duplicate slug.");
                    validationError = true;
                }

                //Return view if error occurs
                if (validationError)
                {
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }
                
                //Assign post's slug if no errors occur
                post.Slug = slug;

                //Adding to the database
                _context.Add(post);
                await _context.SaveChangesAsync();

                //Loop over the incoming tagList of strings
                foreach (var tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = authorId,
                        Text = tagText
                    });
                }

                await _context.SaveChangesAsync();

                //Redirects back to index for Posts
                return RedirectToAction("Details", new { slug = _slugService.UrlFriendly(post.Title) });
            }

            //Reload Blog data to return to Create view in the event that the post fails
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(string? slug)
        {
            if (slug == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Slug == slug);

            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));

            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadTime,ReadyStatus,Image")] Post post, IFormFile newImage, List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Gather existing post from DB
                    var newPost = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);

                    //Record update time
                    newPost.Updated = DateTime.Now;

                    //Check for changes that need to be applied
                    if (newPost.Title != post.Title)
                        newPost.Title = post.Title;

                    if (newPost.Abstract != post.Abstract)
                        newPost.Abstract = post.Abstract;

                    if (newPost.Content != post.Content)
                        newPost.Content = post.Content;

                    if (newPost.ReadyStatus != post.ReadyStatus)
                        newPost.ReadyStatus = post.ReadyStatus;

                    if (newPost.ReadTime != post.ReadTime)
                        newPost.ReadTime = post.ReadTime;

                    //Check unique ReadyStatus
                    var readyUnique = _context.Posts.Where(p => p.ReadyStatus == ReadyStatus.ProductionReady);

                    var newSlug = _slugService.UrlFriendly(post.Title);
                    if (newSlug != newPost.Slug)
                    {
                        if (_slugService.isUnique(newSlug))
                        {
                            newPost.Title = post.Title;
                            newPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "Duplicate title results is slug name collision.");
                            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
                            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
                            return View(post);
                        }
                    }

                    if (newImage is not null)
                    {
                        newPost.ImageData = await _imageService.EncodeImageAsync(newImage);
                        newPost.ContentType = _imageService.ContentType(newImage);
                    }

                    //Remove all tags previously associated with this post
                    _context.Tags.RemoveRange(newPost.Tags);

                    foreach (var tagText in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = newPost.BlogUserId,
                            Text = tagText
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //post.Slug doesn't work?
                return RedirectToAction("Details", new { slug = _slugService.UrlFriendly(post.Title) });
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", post.BlogUserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
