using Microsoft.AspNetCore.Mvc;
using API.Data;
using System.Collections.Generic;
using API.Entities;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{

    public class UsersController : BaseApiController
    {
        public UsersController(DatatContext myDB)
        {
            _myDB = myDB;

        }
        public DatatContext _myDB { get; set; }

        [AllowAnonymous]
        [HttpGet]
        public  async Task < ActionResult<IEnumerable<AppUser>>> GetAllUsers()
        {
            return  await _myDB.Users.ToListAsync();
        }

        [Authorize] 
        [HttpGet("{Id}")]
        public async  Task <ActionResult<AppUser>> GetUser(int Id)
        {
            return await _myDB.Users.FindAsync(Id);
        }
    }
}