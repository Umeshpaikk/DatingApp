using Microsoft.AspNetCore.Mvc;
using API.Data;
using System.Collections.Generic;
using API.Entities;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AppUserController : ControllerBase
    {
        public AppUserController(DatatContext myDB)
        {
            Console.WriteLine("Constructed");
            _myDB = myDB;

        }
        public DatatContext _myDB { get; set; }

        [HttpGet]
        public  async Task < ActionResult<IEnumerable<AppUser>>> GetAllUsers()
        {
            return  await _myDB.Users.ToListAsync();
        }

         
         [HttpGet("{Id}")]
        public async  Task <ActionResult<AppUser>> GetUser(int Id)
        {
            return await _myDB.Users.FindAsync(Id);
        }
    }
}