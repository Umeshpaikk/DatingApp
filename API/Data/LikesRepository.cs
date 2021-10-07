using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        public DatatContext _context { get; }
        public LikesRepository(DatatContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikesDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(user => user.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(likesParams.predicate == "liked")
            {
                likes =  likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUserRef);
            }
            else if(likesParams.predicate == "likedBy")
            {
                likes =  likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUserRef);
            }

            var result = users.Select(user => new LikesDTO
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikesDTO>.CreateAsync(result, likesParams.PageNumber, likesParams.PageSize);

        }

        public async Task<AppUser> GetUsersWithLikes(int userId)
        {
            return await _context.Users
                        .Include(x=>x.LikedUsersBase)
                        .FirstOrDefaultAsync(x=>x.Id==userId);
        }
    }
}