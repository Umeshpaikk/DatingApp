using System.Threading.Tasks;
using API.Entities;

namespace API
{
    public interface ITokenService
    {
        public Task<string> CreateToken(AppUser appUser);
    }
}