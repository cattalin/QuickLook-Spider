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

namespace QuickLook.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        ApplicationDbContext context;
        public AuthController()
        {
            context = new ApplicationDbContext();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody][Required] LoginModel user)
        {
            var entity = context.Users.First(u => u.Username == user.Username);

            if (entity == null)
                return BadRequest(new { Status = "Login Failed" });

            if (user.Username == entity.Username && HashPassword(user.Password, Convert.FromBase64String(entity.Salt)) == entity.PasswordHash)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody]RegisterModel user)
        {
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