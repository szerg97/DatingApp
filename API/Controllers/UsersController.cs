using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly ApplicationDBContext context;

        public UsersController(ApplicationDBContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await context.Users.ToListAsync();
        }

        [Authorize]
        [HttpGet("{uid}")]
        public async Task<ActionResult<AppUser>> GetUser(string uid)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.UID == uid);
        }
    }
}