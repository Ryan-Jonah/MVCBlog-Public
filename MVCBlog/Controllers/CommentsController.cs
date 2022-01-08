using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCBlog.Data;
using MVCBlog.Models;
using MVCBlog.Services;

namespace MVCBlog.Controllers
{
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

        // GET: CommentsS
        public async Task<IActionResult> Index()
        {
            var originalComents = await _context.Comments.ToListAsync();
            return View("Index", originalComents);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("PostId,Body")] Comment comment, string PostSlug)
        {
            if (ModelState.IsValid)
            {
                //Get PK of currently logged in user
                comment.BlogUserId = _userManager.GetUserId(User);
                comment.Created = DateTime.Now;

                //Assign comment level to determine indentation on the front-end
                comment.CommentLevel = 1;

                //Save
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Posts", new { slug = PostSlug }, "commentSection");
            }
            return RedirectToAction("Details", "Posts", new { slug = PostSlug }, "commentSection");
        }

        //BINDING elements associates input from HTML submits to model properties
        //Anything not associated with the model must be defined as a seperate parameter
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> CreateReply([Bind("PostId,ParentCommentId,Body")] Comment replyComment, string PostSlug)
        {
            if (ModelState.IsValid)
            {
                //Get PK of currently logged in user
                replyComment.BlogUserId = _userManager.GetUserId(User);
                replyComment.Created = DateTime.Now;

                //Get parent comment
                var parentComment = await _context.Comments.FindAsync(replyComment.ParentCommentId);

                //Determine comment level in the reply chain 
                replyComment.CommentLevel = await _commentService.GetCommentLevelAsync(replyComment);

                //Add reply to the parent comment
                parentComment.Replies.Add(replyComment);

                //Save
                _context.Add(replyComment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Posts", new { slug = PostSlug }, "commentSection");
            }
            return RedirectToAction("Details", "Posts", new { slug = PostSlug }, "commentSection");
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Body")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var newComment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);
                try
                {
                    newComment.Body = comment.Body;
                    newComment.Updated = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Posts", new { slug = newComment.Post.Slug}, "commentSection");
            }

            return View(comment);
        }

        //POST Comment Moderate Modal
        public async Task<IActionResult> Moderate(int id, [Bind("Id,Body,ModeratedBody,ModerationType")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var newComment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);
                try
                {
                    newComment.ModeratedBody = comment.ModeratedBody;
                    newComment.ModerationType = comment.ModerationType;

                    newComment.ModeratorId = _userManager.GetUserId(User);
                    newComment.Moderated = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Posts", new { slug = newComment.Post.Slug }, "commentSection");
            }

            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.BlogUser)
                .Include(c => c.Moderator)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id, string slug)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            HashSet<Comment> replies = await _commentService.GetCommentRepliesAsync(comment);

            //Remove all replies on delete
            if (replies is not null)
            {
                foreach (var reply in replies)
                {
                    _context.Comments.Remove(reply);
                }
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new { slug }, "commentSections");
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
