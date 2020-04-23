using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationSampleWebApp.DTOs;
using AuthenticationSampleWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationSampleWebApp.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<ShortStudent>();
            list.Add(new ShortStudent
            {
                IdStudent = 1,
                Name = "Andrzej"
            });
            list.Add(new ShortStudent
            {
                IdStudent = 3,
                Name = "Wieslaw"
            });
            return Ok(list);
        }

    }

}