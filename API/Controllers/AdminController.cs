using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRoles")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> getUsersWithRoles()
        {
            var users = await userManager.Users
            .Include(r => r.UserRoles)
            .ThenInclude(u => u.Role)
            .OrderBy(u => u.UserName)
            .Select( u => new {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

            return Ok(users);
        }


        [Authorize(Policy = "ModaratePhotoRoles")]
        [HttpGet("photos-to-moderate")]
        public ActionResult getPhotosForModaration()
        {
            return Ok("Admin or Modarators can access this");
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selRoles = roles.Split(",").ToArray();
            var user = await userManager.FindByNameAsync(username);

            if(user == null) return NotFound("User not found");
            
            var Userroles = await userManager.GetRolesAsync(user);

            var res = await userManager.AddToRolesAsync(user, selRoles.Except(Userroles));

            if(!res.Succeeded) return BadRequest("Failed to add to roles");

            res = await userManager.RemoveFromRolesAsync(user, Userroles.Except(selRoles));

            if(!res.Succeeded) return BadRequest("Failed to remove roles");

            return Ok(await userManager.GetRolesAsync(user));
        }
    }
}