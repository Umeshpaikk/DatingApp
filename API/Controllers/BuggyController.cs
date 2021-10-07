using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        public BuggyController(DatatContext bbcontext)
        {
            Bbcontext = bbcontext;
        }

        public DatatContext Bbcontext { get; }

        [Authorize]
        [HttpGet("auth")]
        //401
        public  ActionResult<string> getsecreats()
        {
            return  "returning secreats"; 
        }

        //404
        [HttpGet("not-found")]
        public  ActionResult<AppUser> getNotFound()
        {
            var thing = Bbcontext.Users.Find(-1);
            if(thing == null) return NotFound();

            return Ok();
        }

        [HttpGet("server-error")]
        //500
        public  ActionResult<string> getServerError()
        {
            var thing = Bbcontext.Users.Find(-1);
            var thing2return = thing.ToString();
            return thing2return;
        }

        [HttpGet("bad-request")]
        //400
        public  ActionResult<string> getBadRequest()
        {
            return  BadRequest("Bad request returned");
        }


    }
}