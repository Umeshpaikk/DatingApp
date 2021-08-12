using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        public AccountController(DatatContext dbContext, ITokenService tokenService)
        {
            _tokenService = tokenService;
            DbContext = dbContext;
        }
        public DatatContext DbContext { get; set; }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> register(RegisterDTO registerDTO)
        {
            if (await IsUserExist(registerDTO.username))
                return BadRequest("User already in Use");

            using var hmac = new HMACSHA512();

            var User = new AppUser
            {
                UserName = registerDTO.username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.password)),
                PasswordSalt = hmac.Key
            };

            DbContext.Users.Add(User);
            var result = await DbContext.SaveChangesAsync();

            return new UserDTO
            {
                Username = User.UserName,
                Token = _tokenService.CreateToken(User)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> login(LoginDTO loginDTO)
        {
            var User = await DbContext.Users.FirstOrDefaultAsync(x => x.UserName == loginDTO.Username);

            if (User == null) return Unauthorized("Invalid User");

            using var hmac = new HMACSHA512(User.PasswordSalt);
            var PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < PasswordHash.Length && i < User.PasswordHash.Length; i++)
            {
                if (User.PasswordHash[i] != PasswordHash[i])
                    return Unauthorized("Invalid Password");
            }


            return new UserDTO
            {
                Username = User.UserName,
                Token = _tokenService.CreateToken(User)
            };
        }

        private async Task<bool> IsUserExist(string UserName)
        {
            return await DbContext.Users.AnyAsync(x => x.UserName == UserName.ToLower());

        }
    }
}