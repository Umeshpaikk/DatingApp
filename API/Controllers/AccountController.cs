using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        public IMapper _map { get; }
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper map)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _map = map;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> register(RegisterDTO registerDTO)
        {
            if (await IsUserExist(registerDTO.username))
                return BadRequest("User already in Use");

            // using var hmac = new HMACSHA512();

            AppUser User = _map.Map<AppUser>(registerDTO);

            User.UserName = registerDTO.username.ToLower();
            // User.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.password));
            // User.PasswordSalt = hmac.Key;

            var result = await _userManager.CreateAsync(User, registerDTO.password);

            if(!result.Succeeded) return Unauthorized("Unauthorized");

            var roleRes = await _userManager.AddToRoleAsync(User, "Member");

            if(!roleRes.Succeeded) return BadRequest("Unable to add roles");
            
            return new UserDTO
            {
                Username = User.UserName,
                Token = await _tokenService.CreateToken(User),
                KnownAs = User.KnownAs,
                Gender = User.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> login(LoginDTO loginDTO)
        {
            var User = await _userManager.Users.
             Include(p => p.Photos)
            .FirstOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());

            if (User == null) return Unauthorized("Invalid User");

            // using var hmac = new HMACSHA512(User.PasswordSalt);
            // var PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            // for (int i = 0; i < PasswordHash.Length && i < User.PasswordHash.Length; i++)
            // {
            //     if (User.PasswordHash[i] != PasswordHash[i])
            //         return Unauthorized("Invalid Password");
            // }

            var result = await _signInManager
            .CheckPasswordSignInAsync(User, loginDTO.Password, false);

            if(!result.Succeeded) return Unauthorized("Invalid Password");

            return new UserDTO
            {
                Username = User.UserName,
                Token = await _tokenService.CreateToken(User),
                PhotoUrl = User.Photos?.FirstOrDefault(x => x.IsMain == true)?.Url,
                KnownAs = User.KnownAs,
                Gender = User.Gender
            };
        }

        private async Task<bool> IsUserExist(string UserName)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == UserName.ToLower());

        }
    }
}