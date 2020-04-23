using AuthenticationSampleWebApp.DTOs;
using AuthenticationSampleWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AuthenticationSampleWebApp.Handlers
{
    public class BasicAuthHandlers : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthHandlers(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
            //IStudentDbService service) : base(options, logger, encoder, clock)
        {

        }

        private SqlServerStudentDbService _service;
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing authorization haeder");



            //"Autorization: Basic xxxxx"
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter); //xxxxx => bajty -> jan123:haslo123
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");

            if(credentials.Length != 2)
                return AuthenticateResult.Fail("Incorrect authorization header value");

            _service = new SqlServerStudentDbService();

            var login = new LoginRequestDto
            {
                Login = credentials[0],
                Haslo = credentials[1]
            };

            if (!_service.CheckUserPassword(login))
                return AuthenticateResult.Fail("User or password incorrect");


            //TODO chceck credentials id DB

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, credentials[0]),
                new Claim(ClaimTypes.Role, "employee"),
                new Claim(ClaimTypes.Role, "student"),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name); //np Basic, ...
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
