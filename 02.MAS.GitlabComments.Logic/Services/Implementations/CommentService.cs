﻿namespace MAS.GitlabComments.Logic.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Exceptions;
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.Logic.Models;

    /// <summary>
    /// Service for managing <see cref="Comment"/>
    /// </summary>
    public class CommentService : ICommentService
    {
        /// <summary>
        /// Data provider of <see cref="Comment"/> entities
        /// </summary>
        private IDataProvider<Comment> CommentsDataProvider { get; }

        /// <summary>
        /// Data provider of <see cref="StoryRecord"/> entities
        /// </summary>
        private IDataProvider<StoryRecord> StoryRecordsDataProvider { get; }

        /// <inheritdoc cref="IApplicationSettings"/>
        private IApplicationSettings ApplicationSettings { get; }

        /// <inheritdoc cref="ISystemVariableProvider"/>
        private ISystemVariableProvider SystemVariableProvider { get; }

        /// <summary>
        /// Initializing <see cref="CommentService"/>
        /// </summary>
        /// <param name="commentsDataProvider">Comments data provider</param>
        /// <param name="storyRecordsDataProvider">Story records data provider</param>
        /// <param name="applicationSettings">Application settings</param>
        /// <param name="systemVariableProvider">System variable data provider</param>
        /// <exception cref="ArgumentNullException">Parameter commentsDataProvider is null</exception>
        /// <exception cref="ArgumentNullException">Parameter storyRecordsDataProvider is null</exception>
        public CommentService(
            IDataProvider<Comment> commentsDataProvider,
            IDataProvider<StoryRecord> storyRecordsDataProvider,
            IApplicationSettings applicationSettings,
            ISystemVariableProvider systemVariableProvider
        )
        {
            CommentsDataProvider = commentsDataProvider ?? throw new ArgumentNullException(nameof(commentsDataProvider));
            StoryRecordsDataProvider = storyRecordsDataProvider ?? throw new ArgumentNullException(nameof(storyRecordsDataProvider));
            ApplicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
            SystemVariableProvider = systemVariableProvider ?? throw new ArgumentNullException(nameof(systemVariableProvider));
        }

        /// <summary>
        /// Add comment by specified values
        /// </summary>
        /// <param name="addCommentModel">Comment values</param>
        /// <exception cref="ArgumentNullException">Parameter addCommentModel is null</exception>
        /// <exception cref="ArgumentNullException">Message isn't specified</exception>
        /// <returns>Identifier value of new created comment</returns>
        public Guid Add(AddCommentModel addCommentModel)
        {
            if (addCommentModel == null)
            {
                throw new ArgumentNullException(nameof(addCommentModel));
            }

            if (string.IsNullOrEmpty(addCommentModel.Message))
            {
                throw new ArgumentNullException(nameof(AddCommentModel.Message));
            }

            var number = string.Format(
                ApplicationSettings.CommentNumberTemplate,
                SystemVariableProvider.GetValue<int>("LastCommentNumber") + 1
            );

            var newId = CommentsDataProvider.Add(
                new Comment
                {
                    AppearanceCount = 1,
                    Message = addCommentModel.Message,
                    CommentWithLinkToRule = addCommentModel.CommentWithLinkToRule,
                    Description = addCommentModel.Description,
                    Number = number,
                }
            );

            return newId;
        }

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <returns>All comments</returns>
        public IEnumerable<CommentModel> Get()
        {
            return
                CommentsDataProvider
                    .Select<CommentModel>()
                    .OrderByDescending(x => x.AppearanceCount)
                    .ToList();
        }

        /// <summary>
        /// Update specified comment by values
        /// </summary>
        /// <param name="updateCommentModel">Comment new values</param>
        /// <exception cref="ArgumentNullException">Parameter updateCommentModel is null</exception>
        /// <exception cref="ArgumentNullException">Message isn't specified</exception>
        /// <exception cref="ArgumentNullException">Parameter commentId is default</exception>
        /// <exception cref="EntityNotFoundException">Comment not found</exception>
        public void Update(UpdateCommentModel updateCommentModel)
        {
            if (updateCommentModel == null)
            {
                throw new ArgumentNullException(nameof(updateCommentModel));
            }
            if (string.IsNullOrEmpty(updateCommentModel.Message))
            {
                throw new ArgumentNullException(nameof(UpdateCommentModel.Message));
            }

            GetCommentWithWithChecking(updateCommentModel.Id);

            var newValues = new Dictionary<string, object>
            {
                { nameof(Comment.Message), updateCommentModel.Message }
            };

            if (!string.IsNullOrEmpty(updateCommentModel.Description))
            {
                newValues.Add(nameof(Comment.Description), updateCommentModel.Description);
            }
            if (!string.IsNullOrEmpty(updateCommentModel.CommentWithLinkToRule))
            {
                newValues.Add(nameof(Comment.CommentWithLinkToRule), updateCommentModel.CommentWithLinkToRule);
            }

            CommentsDataProvider.Update(updateCommentModel.Id, newValues);
        }

        /// <summary>
        /// Increment <see cref="Comment.AppearanceCount"/> property of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        /// <exception cref="ArgumentNullException">Parameter commentId is default</exception>
        /// <exception cref="EntityNotFoundException">Comment not found</exception>
        public void Increment(Guid commentId)
        {
            var entity = GetCommentWithWithChecking(commentId);

            var newValues = new Dictionary<string, object>
            {
                {  nameof(Comment.AppearanceCount), entity.AppearanceCount + 1 },
            };

            CommentsDataProvider.Update(commentId, newValues);

            StoryRecordsDataProvider.Add(new StoryRecord { IsIncrementAction = true, CommentId = commentId });
        }

        /// <summary>
        /// Delete comments by specifying their identifiers
        /// </summary>
        /// <param name="commentIds">Array of comment identifiers</param>
        /// <exception cref="ArgumentNullException">Parameter commentIds is null</exception>
        public void Delete(params Guid[] commentIds)
        {
            if (commentIds == null)
            {
                throw new ArgumentNullException(nameof(commentIds));
            }

            commentIds = commentIds.Where(x => x != default).ToArray();

            if (commentIds.Any())
            {
                CommentsDataProvider.Delete(commentIds);
            }
        }

        #region Not public API

        /// <summary>
        /// Get comment with validation by specified identifier
        /// </summary>
        /// <param name="commentId">Comment identifier value</param>
        /// <exception cref="ArgumentNullException">Parameter commentId is default</exception>
        /// <exception cref="EntityNotFoundException">Comment not found</exception>
        /// <returns>Found comment</returns>
        private Comment GetCommentWithWithChecking(Guid commentId)
        {
            if (commentId == default)
            {
                throw new ArgumentNullException(nameof(commentId));
            }

            var entity = CommentsDataProvider.Get(commentId);

            if (entity == null)
            {
                throw new EntityNotFoundException(nameof(Comment), commentId);
            }

            return entity;
        }

        #endregion
    }
}
