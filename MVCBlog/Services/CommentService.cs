using Microsoft.EntityFrameworkCore;
using MVCBlog.Data;
using MVCBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Services
{
    public class CommentService : ICommentService
    {
        //GetCommentRepliesAsync() variable
        private HashSet<Comment> replySet { get; set; } = new();

        private readonly ApplicationDbContext _dbContext;

        public CommentService(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public HashSet<Comment> FilterParentComments(ICollection<Comment> comments)
        {
            HashSet<Comment> parentComments = new HashSet<Comment>();

            foreach (Comment comment in comments)
            {
                if(comment.ParentCommentId is null)
                {
                    parentComments.Add(comment);
                }
            }

            return parentComments;
        }

        public async Task<Comment>GetTopLevelCommentAsync(Comment replyComment)
        {
            //Exit Case
            if (replyComment.ParentCommentId is null)
            {
                return replyComment;
            }

            //Recursively call the function as long as a parent comment is found
            Comment parentComment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == replyComment.ParentCommentId);
            return await GetTopLevelCommentAsync(parentComment);
        }

        public async Task<int> GetCommentLevelAsync(Comment comment, int startIndex = 1)
        {
            //Exit Case
            if (comment.ParentCommentId is null)
            {
                return startIndex;
            }

            //Increase index with each recursive call when a parent is found
            Comment parentComment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == comment.ParentCommentId);
            return await GetCommentLevelAsync(parentComment, startIndex + 1);
        }

        public async Task<HashSet<Comment>> GetCommentRepliesAsync(Comment comment)
        {
            //Queries database for replies during each level of recursion
            comment = await _dbContext.Comments.Include(c => c.Replies).FirstOrDefaultAsync(c => c.Id == comment.Id);

            //Exit case
            if (comment.Replies.Count == 0)
            {
                return new HashSet<Comment>();
            }

            //Recursive call when replies are found
            foreach (Comment reply in comment.Replies)
            {
                //Class level container for replies
                replySet.Add(reply);

                await GetCommentRepliesAsync(reply);
            }

            return replySet;
        }
    }
}
