using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUsersWithLikes(int userId);
        Task<PagedList<LikesDTO>> GetUserLikes(LikesParams likesParams);
    }
}