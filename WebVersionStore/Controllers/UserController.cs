using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebVersionStore.Models;

namespace WebVersionStore.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private WebVersionControlContext _database;
        public UserController(WebVersionControlContext _database) 
        {
            this._database = _database;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToActionPermanent(nameof(List));
        }

        [HttpGet]
        public ActionResult List()
        {
            return Json(new ResponceModel(
                (from user in _database.Users 
                 select new { user.Login })
                .Take(100).ToList()
                ));
        }

        [HttpGet]
        public ActionResult Details(string login)
        {
            var res = _database.Users.Find(login);
            if (res == null) return Json(new ResponceModel("Error", "User not found"));
            return Json(res);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(User user)
        {
            if (!ModelState.IsValid)
                return Json(new ResponceModel("Error", "Missing required value"));

            _database.Users.Add(user);

            if(await _database.SaveChangesAsync() == 0)
                return Json(new ResponceModel("Error", "Saving failed"));

            return RedirectToActionPermanent(nameof(Details), user.Login);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(User user)
        {
            /*var inputType = user.GetType();
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
                return Json(new ResponceModel("Error", "Missing required value"));

            _database.Users.Update(user);
            await _database.SaveChangesAsync();
            return RedirectToActionPermanent(nameof(Details), new { user.Login });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string login)
        {
            var user = await _database.Users.FindAsync(login);
            if (user != null)
            {
                _database.Users.Remove(user);
                await _database.SaveChangesAsync();
            }
            return Json(new ResponceModel("User deleted"));
        }
    }
}
