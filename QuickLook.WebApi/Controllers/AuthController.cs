using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using QuickLook.WebApi.Entities;
using QuickLook.WebApi.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace QuickLook.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _config;
        private ApplicationDbContext context;

        public AuthController(IConfiguration config)
        {
            _config = config;
            context = new ApplicationDbContext();
        }


        [HttpGet]
        [Route("test")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Status = "Please complete all the fields" });

            var entity = context.Users.FirstOrDefault(u => u.Username == user.Username);

            if (entity == null)
                return Unauthorized(new { Status = "Username or password is wrong" });

            if (user.Username == entity.Username && HashPassword(user.Password, Convert.FromBase64String(entity.Salt)) == entity.PasswordHash)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Issuer"],
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddDays(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    Status = "Successful login",
                    Token = tokenString,
                    Username = user.Username,
                    Email = entity.Email
                });
            }
            else
            {
                return Unauthorized(new { Status = "Username or password is wrong" });
            }
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody]RegisterModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Status = "Please complete all the fields" });

            var exiting = context.Users.Any(u => u.Username == user.Username || u.Email == user.Email);

            if (exiting)
                return BadRequest(new { Status = "Username or Email taken" });

            var salt = GenerateSalt();

            var entity = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = user.Email,
                Username = user.Username,
                Salt = Convert.ToBase64String(salt),
                PasswordHash = HashPassword(user.Password, salt),
            };

            context.Users.Add(entity);

            context.SaveChanges();

            return Ok(new { Status = "Successfully created" });
        }

        // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
        private string HashPassword(string password, byte[] salt)
        {
            var bytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(bytes);
        }

        // generate a 128-bit salt using a secure PRNG
        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }
    }
}