﻿namespace MAS.GitlabComments.Services
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Models;

    /// <summary>
    /// Service for managing <see cref="Comment"/>
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// Add comment by specified values
        /// </summary>
        /// <param name="addCommentModel">Comment values</param>
        public void Add(AddCommentModel addCommentModel);

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <returns>All comments</returns>
        public IEnumerable<CommentModel> Get();

        /// <summary>
        /// Get comment item by specifying it's identifier
        /// </summary>
        /// <param name="commentId">Comment identifier value</param>
        /// <returns>Comment model</returns>
        public CommentModel Get(Guid commentId);

        /// <summary>
        /// Get description of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        /// <returns>Description if specified; otherwise <see cref="string.Empty"/></returns>
        public string GetDescription(Guid commentId);

        /// <summary>
        /// Get description of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        /// <returns>Description if specified; otherwise <see cref="string.Empty"/></returns>
        public void Delete(params Guid[] commentIds);

        /// <summary>
        /// Increment <see cref="Comment.AppearanceCount"/> property of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        public void Increment(Guid commentId);

        /// <summary>
        /// Update specified comment by values
        /// </summary>
        /// <param name="updateCommentModel">Comment new values</param>
        public void Update(UpdateCommentModel updateCommentModel);
    }
}
