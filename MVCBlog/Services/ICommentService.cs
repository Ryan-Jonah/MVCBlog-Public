using MVCBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Services
{
    public interface ICommentService
    {
        /// <summary>
        /// Generates filtered collection of parent comments from the given collection of comments
        /// </summary>
        /// <param name="comments">Collection of comments to filter</param>
        /// <returns>Parent comments</returns>
        HashSet<Comment> FilterParentComments(ICollection<Comment> comments);

        /// <summary>
        /// Determines the top level comment for a given comment within the reply chain
        /// </summary>
        /// <param name="replyComment">Comment within the reply chain</param>
        /// <returns>The root comment of the reply chain</returns>
        Task<Comment> GetTopLevelCommentAsync(Comment replyComment);

        /// <summary>
        /// Returns an integer defining the index of the given comment within the reply chain
        /// </summary>
        /// <param name="comment">The comment to index</param>
        /// <param name="startIndex">Initial value used to track iteration through each level of a reply</param>
        /// <returns>Comment's level within the reply chain</returns>
        Task<int> GetCommentLevelAsync(Comment comment, int startIndex = 1);

        /// <summary>
        /// Generates collection of recursive replies for a given comment
        /// </summary>
        /// <param name="comment">Top level comment</param>
        /// <returns>Hashset of reply tree</returns>
        Task<HashSet<Comment>> GetCommentRepliesAsync(Comment comment);
    }
}
