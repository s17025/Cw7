using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationSampleWebApp.DTOs;
using AuthenticationSampleWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationSampleWebApp.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;
        private static IConfiguration _configuration;

        public EnrollmentsController(IStudentDbService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            _service.EnrollStudent(request);
            return Ok();
        }

        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudent(PromoteStudentRequest request)
        {
            _service.PromoteStudents(request);
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto request)
        {
            var salt = _service.GetSalt(request.Login);
            request.Haslo = EncryptPass(request.Haslo, salt);

            if (!_service.CheckUserPassword(request))
                return Ok("User or password incorrect");

            var token = CreateToken();
            var refToken = Guid.NewGuid();

            _service.AddToken(request.Login, refToken.ToString());

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = refToken
            });
        }

        [HttpPost("refresh-Token/{refToken}")]
        public IActionResult RefreshToken(string refToken)
        {
            if (!_service.CheckToken(refToken))
                return Ok("No such token");

            var newToken = CreateToken();
            var newRefToken = Guid.NewGuid();

            _service.ChangeToken(refToken, newRefToken.ToString());

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(newToken),
                refreshToken = newRefToken
            });



        }

        public static JwtSecurityToken CreateToken()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "employee"),
                new Claim(ClaimTypes.Role, "student")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return token;
        }

        public static string EncryptPass(string pass, string saltPass)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: pass,
                salt: Encoding.UTF8.GetBytes(saltPass),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 128 / 8);
            return Convert.ToBase64String(valueBytes);
        }
    }
}