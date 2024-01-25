using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebVersionStore.Models;

namespace WebVersionStore.Controllers
{
    [Route("auth")]
    public class AuthenticationController : Controller
    {
        WebVersionControlContext _database;
        public AuthenticationController(WebVersionControlContext db)
        {
            _database = db;
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody]AuthRequestModel model)
        {
            //model.Password = encrypted model.Password
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = _database.Users.Find(model.Login);
            if (user == null) return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();


            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim("Login", user.Login),
                new Claim("Password", user.Password)
            });
            var tokendata = new SecurityTokenDescriptor
            {
                Subject = identity,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("testkeyasdfxzcvzxcv12345678912356465445645564")),
                        SecurityAlgorithms.HmacSha256Signature),//TODO swap for smth better
                Issuer = "localhost"
            };
            var token = tokenHandler.CreateToken(tokendata);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                Domain = Request.Host.Value,
                Secure = true,
                Path = "/"
            };
            Response.Cookies.Append("jwt", tokenHandler.WriteToken(token), cookieOptions);
            return Ok();
        }
    }
}
