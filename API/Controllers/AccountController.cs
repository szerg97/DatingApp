using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ApplicationDBContext context;
        private readonly ITokenService tokenService;

        public AccountController(ApplicationDBContext context, ITokenService tokenService)
        {
            this.context = context;
            this.tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
        {
            if (await UserExists(dto.UserName)) return BadRequest("Username is taken.");

            //using a dispose miatt
            using var hmac = new HMACSHA512();

            var user = new AppUser()
            {
                UID = Guid.NewGuid().ToString(),
                UserName = dto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new UserDto() { UserName = user.UserName, Token = this.tokenService.CreateToken(user)};
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == dto.UserName);

            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password.");
            }

            return new UserDto() { UserName = user.UserName, Token = this.tokenService.CreateToken(user) };
        }

        private async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}