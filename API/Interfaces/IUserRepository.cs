using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        public void update(AppUser user);
        //public Task<bool> SaveAllAsync();
        public Task<IEnumerable<AppUser>> GetAllUsersAsync();
        public Task<PagedList<MemberDTO>> GetAllMembersAsync(UserParams userparams);
        public Task<AppUser> GetUserByIDAsync(int Id);
        public Task<AppUser> GetUserByNameAsync(string Name);

        public Task<MemberDTO> GetMemberByNameAsync(string Name);
        
    }
}