﻿namespace MAS.GitlabComments.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.WebApi.Attributes;
    using MAS.GitlabComments.WebApi.Models;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.Logic.Models;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [UseReadOnlyMode]
    [Route("api/comments")]
    public class CommentsApiController : ControllerBase
    {
        /// <summary>
        /// Logger to store error information
        /// </summary>
        private ILogger<CommentsApiController> Logger { get; }

        /// <summary>
        /// Service for comments managing
        /// </summary>
        private ICommentService CommentService { get; }

        /// <summary>
        /// Initialize <see cref="CommentsApiController"/>
        /// </summary>
        /// <param name="logger">Logger to store error information</param>
        /// <param name="commentService">Service for comments managing</param>
        /// <exception cref="ArgumentNullException">Parameter commentService is null</exception>
        public CommentsApiController(
            ILogger<CommentsApiController> logger,
            ICommentService commentService
        )
        {
            Logger = logger;
            CommentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }

        /// <summary>
        /// Add comment by specified values
        /// </summary>
        /// <param name="addCommentModel">Comment values</param>
        [HttpPost("add")]
        public BaseServiceResult<Guid> Add([FromBody] AddCommentModel addCommentModel)
        {
            try
            {
                var newId = CommentService.Add(addCommentModel);

                return BaseServiceResult<Guid>.Success(newId);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, "Trying to: add comment.");
                return BaseServiceResult<Guid>.Error(e);
            }
        }

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <returns>Service perform operation result</returns>
        [AllowInReadOnly]
        [HttpGet("getAll")]
        public BaseServiceResult<IEnumerable<CommentModel>> Get()
        {
            try
            {
                var result = CommentService.Get();

                return BaseServiceResult<IEnumerable<CommentModel>>.Success(result);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, "Trying to: Get all comments");
                return BaseServiceResult<IEnumerable<CommentModel>>.Error(e);
            }
        }

        /// <summary>
        /// Increment <see cref="Comment.AppearanceCount"/> property of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        [HttpPost("increment")]
        public BaseServiceResult Increment([FromBody] Guid commentId)
        {
            try
            {
                CommentService.Increment(commentId);

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Trying to: Incrementing \"{commentId}\".");
                return BaseServiceResult.Error(e);
            }
        }

        /// <summary>
        /// Update specified comment by values
        /// </summary>
        /// <param name="updateCommentModel">Comment new values</param>
        [HttpPost("update")]
        public BaseServiceResult Update([FromBody] UpdateCommentModel updateCommentModel)
        {
            try
            {
                CommentService.Update(updateCommentModel);

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Trying to: Update comment \"{updateCommentModel?.Id}\".");
                return BaseServiceResult.Error(e);
            }
        }

        /// <summary>
        /// Delete comments by specified identifiers
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        [HttpPost("delete")]
        public BaseServiceResult Delete([FromBody] Guid commentId)
        {
            try
            {
                CommentService.Delete(commentId);

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Trying to: Delete comment \"{commentId}\".");
                return BaseServiceResult.Error(e);
            }
        }

        /// <summary>
        /// Get incomplete comments data
        /// </summary>
        [HttpGet("getIncomplete")]
        public BaseServiceResult<IEnumerable<IncompleteCommentData>> GetIncomplete()
        {
            try
            {
                var result = CommentService.GetIncomplete();

                return BaseServiceResult<IEnumerable<IncompleteCommentData>>.Success(result);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, "Get incomplete comments");
                return BaseServiceResult<IEnumerable<IncompleteCommentData>>.Error(e);
            }
        }

        /// <summary>
        /// Update incomplete comments
        /// </summary>
        [HttpPost("updateIncomplete")]
        public BaseServiceResult UpdateIncomplete()
        {
            try
            {
                CommentService.UpdateIncomplete();

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger?.LogError(e, "Updating incomplete comments failed");
                return BaseServiceResult.Error(e);
            }
        }

        /// <summary>
        /// Update comment table definition
        /// </summary>
        [HttpPost("updateCommentTable")]
        public BaseServiceResult UpdateCommentTable()
        {
            try
            {
                CommentService.MakeNumberColumnUnique();

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger?.LogError(e, "Updating comment table with unique constraint for Number column failed");
                return BaseServiceResult.Error(e);
            }
        }

        /// <summary>
        /// Update comment table definition
        /// </summary>
        [HttpGet("canUpdateCommentTable")]
        public BaseServiceResult<bool> CheckCanUpdateCommentTable()
        {
            try
            {
                var result = CommentService.CanMakeNumberColumnUnique();

                return BaseServiceResult<bool>.Success(result);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, "Checking ability of updating comment table with unique constraint for Number column failed");
                return BaseServiceResult<bool>.Error(e);
            }
        }
    }
}
