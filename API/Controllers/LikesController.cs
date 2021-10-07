using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork unitOfWork;
        public LikesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

        }


        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceuserId = User.getUserId();
            var LikedUser = await unitOfWork.UserRepository.GetUserByNameAsync(username);
            var sourceUser = await unitOfWork.LikesRepository.GetUsersWithLikes(sourceuserId);

            if (LikedUser == null) return BadRequest("Invalid ID");

            if (sourceUser.UserName == username) return BadRequest("Cant like yourself");

            var userLike = await unitOfWork.LikesRepository.GetUserLike(sourceuserId, LikedUser.Id);

            if (userLike != null) return BadRequest("Already Liked");

            userLike = new Entities.UserLike()
            {
                SourceUserId = sourceuserId,
                LikedUserId = LikedUser.Id,
                // LikedUserRef = LikedUser,
                // SourceUserRef = sourceUser
            };

            sourceUser.LikedUsersBase.Add(userLike);
            if (await unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Failed to Like");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikesDTO>>> getUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.getUserId();
            var result = await unitOfWork.LikesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeaders(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);

            return Ok(result);
        }
    }

}