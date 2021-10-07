using Microsoft.AspNetCore.Mvc;
using API.Data;
using System.Collections.Generic;
using API.Entities;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using AutoMapper;
using API.DTO;
using System.Security.Claims;
using API.Extentions;
using Microsoft.AspNetCore.Http;
using API.Helpers;

namespace API.Controllers
{

    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public IUnitOfWork UnitOfWork { get; }

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            this.UnitOfWork = unitOfWork;
            _photoService = photoService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IEnumerable<MemberDTO>> GetAllUsers([FromQuery] UserParams userparams)
        {
            var user = await UnitOfWork.UserRepository.GetMemberByNameAsync(User.getUserName());
            userparams.CurrentUsername = user.Username;

            if (string.IsNullOrEmpty(userparams.Gender))
            {
                userparams.Gender = (user.Gender == "male") ? "female" : "male";
            }

            var users = await UnitOfWork.UserRepository.GetAllMembersAsync(userparams);

            Response.AddPaginationHeaders(users.CurrentPage, users.PageSize,
                        users.TotalCount, users.TotalPages);

            return users;
        }

        [Authorize(Roles = "Member")]
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            var user = await UnitOfWork.UserRepository.GetMemberByNameAsync(username);
            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateDTO(MemberUpdateDTO memberUpdate)
        {
            var username = User.getUserName();
            var user = await UnitOfWork.UserRepository.GetUserByNameAsync(username);

            _mapper.Map(memberUpdate, user);
            UnitOfWork.UserRepository.update(user);

            if (await UnitOfWork.Complete())
                return NoContent();
            else
                return BadRequest();
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user = await UnitOfWork.UserRepository.GetUserByNameAsync(User.getUserName());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await UnitOfWork.Complete())
            {
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDTO>(photo));
            }
            return BadRequest("Problem addding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await UnitOfWork.UserRepository.GetUserByNameAsync(User.getUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await UnitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await UnitOfWork.UserRepository.GetUserByNameAsync(User.getUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await UnitOfWork.Complete()) return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}