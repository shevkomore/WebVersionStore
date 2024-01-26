using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebVersionStore.Models;
using WebVersionStore.Models.Local;

namespace WebVersionStore.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        WebVersionControlContext _database;
        JwtSettings _jwtSettings;
        IPasswordHasher<User> _passwordHasher;
        public UserController(WebVersionControlContext _database, IOptions<JwtSettings> _jwtSettings, IPasswordHasher<User> hasher) 
        {
            this._database = _database;
            this._jwtSettings = _jwtSettings.Value;
            this._passwordHasher = hasher;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToActionPermanent(nameof(List));
        }

        [HttpGet]
        public ActionResult List()
        {
            return Json(
                (from user in _database.Users 
                 select new { user.Login })
                .Take(100).ToList()
                );
        }

        [HttpGet]
        public ActionResult Details(string login)
        {
            var res = _database.Users.Find(login);
            if (res == null) return NotFound();
            return Json(res);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Create([FromBody]User user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            user.Password = _passwordHasher.HashPassword(user, user.Password);

            _database.Users.Add(user);

            if(await _database.SaveChangesAsync() == 0)
                //Something's wrong on server side
                return StatusCode(500);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Edit([FromBody]User user)
        {
            /*  Variation that allows sending partial user data (does not work: using object gives no info about stored data)
            var inputType = user.GetType();
            if (user == null || inputType.GetProperty("Login") == null)
                return Json(new { status = "error", message = "Missing user login" });
            var databaseUser = _database.Users.Find(inputType.GetProperty("Login").GetValue(user));
            if(databaseUser == null)
                return Json(new { status = "error", message = "User not found" });

            foreach(var param in inputType.GetProperties())
                if(param.Name != "Login" 
                    && param.CanWrite 
                    && param.GetValue(user) != null)
                        param.SetValue(databaseUser, param.GetValue(user));

            _database.SaveChanges();

            return RedirectToActionPermanent(nameof(Details), inputType.GetProperty("Login").GetValue(user));*/

            if (!ModelState.IsValid)
                return BadRequest();

            _database.Users.Update(user);
            await _database.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string login)
        {
            var user = await _database.Users.FindAsync(login);
            if (user != null)
            {
                _database.Users.Remove(user);
                await _database.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponceModel>> Login([FromBody] AuthRequestModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            

            var userData = await _database.Users.FindAsync(user.Login);
            if (userData == null) 
                return NotFound();

            var passwordCheck = _passwordHasher.VerifyHashedPassword(userData, userData.Password, user.Password);
            if (passwordCheck == PasswordVerificationResult.Failed)
                return NotFound();

            AuthResponceModel responce = new AuthResponceModel(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokendata = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Login)
                }),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_jwtSettings.SecretBytes),
                    SecurityAlgorithms.HmacSha256Signature),//TODO swap for smth better
            };
            responce.Token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokendata));
            return responce;
        }
    }
}
