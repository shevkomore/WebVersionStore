using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using WebVersionStore.Handlers;
using WebVersionStore.Models;
using WebVersionStore.Models.Database;

namespace WebVersionStore.Controllers
{
    [Authorize]
    public class RepositoryController : Controller
    {
        WebVersionControlContext _database;
        public RepositoryController(WebVersionControlContext database)
        {
            _database = database;
        }
        //Non-specific requests, and non-action methods (private/with NonActionAttribute)
        #region Generic
        [HttpGet]
        public ActionResult Index() => RedirectToActionPermanent(nameof(List));

        Repository? FindRepository(Guid repositoryId)
        {
            return _database.Repositories.Find(repositoryId);
        }
        Repository? FindRepository(Guid repositoryId, RepositoryAccessLevel level, IIdentity? user)
        {
            if (user == null) return null;
            var repository = FindRepository(repositoryId);
            if (repository == null) return null;
            if (!user.CanAccess(repository, level)) return null;
            return repository;
        }
        #endregion
        //Requests regarding the list of all repositories available to user
        #region RepositoriesList
        [HttpGet]
        public ActionResult List()
        {
            if (HttpContext.User.Identity == null)
                return BadRequest();

            var list = from repo in _database.Repositories
                       where HttpContext.User.Identity.CanAccess(repo, RepositoryAccessLevel.VIEW) 
                       select repo;
            return Json(list.ToList());
        }

        [HttpGet("Search")]
        public ActionResult ListWithNameSnippet(string name)
        {
            //Unnecessary; might delete later
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult ListOwned()
        {
            if (HttpContext.User.Identity == null)
                return BadRequest();

            var list = from repo in _database.Repositories
                       where repo.Author == HttpContext.User.Identity.Name
                       select repo;
            return Json(list.ToList());
        }

        [HttpGet]
        public ActionResult ListAccess(RepositoryAccessSettingsModel accessSettings)
        {
            if (HttpContext.User.Identity == null)
                return BadRequest();

            var list = from repo in _database.Repositories
                       where repo.UserRepositoryAccesses.Any(access =>
                       /*   Short explanation:
                        *   - If a field is "marked"(i.e. true) in accessSettings,
                        *   the corresponding condition is added to our check.
                        *   - Only the added conditions are tested; otherwise
                        *   the state is not filtered.
                        *   - Since the LINQ request condition is converted to SQL,
                        *   the "normal" algorithm for this throws an error;
                        *   this is why the whole thing is written in a form of 
                        *   a boolean operation
                        */
                            //          Structure:
                            // \/   Trigger              \/ Condition
                             (!accessSettings.IsOwner  | repo.Author == HttpContext.User.Identity.Name)
                           & (!accessSettings.CanView  | access.CanView)
                           & (!accessSettings.CanEdit  | access.CanEdit)
                           & (!accessSettings.CanAdd   | access.CanAdd)
                           & (!accessSettings.CanRemove| access.CanRemove)
                           )
                       select repo;
            return Json(list.ToList());
        }
        #endregion
        //Requests regarding a specific repository. Generally call FindRepository (in General)
        #region Repository
        [HttpGet]
        public ActionResult Details(Guid repositoryId)
        {
            var repository = FindRepository(repositoryId, RepositoryAccessLevel.VIEW, HttpContext.User.Identity);
            if (repository == null) return NotFound();
            throw new NotImplementedException();

        }

        [HttpPost]
        public ActionResult Create(RepositoryDisplaySettingsModel model)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult Edit(Guid repositoryId, RepositoryDisplaySettingsModel model)
        {
            var repository = FindRepository(repositoryId, RepositoryAccessLevel.EDIT, HttpContext.User.Identity);
            if (repository == null) return NotFound();
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult GrantAccess(Guid repositoryId, RepositoryAccessLevel type) 
        {
            var repository = FindRepository(repositoryId, RepositoryAccessLevel.EDIT, HttpContext.User.Identity);
            if (repository == null) return NotFound();
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult RevokeAccess(Guid repositoryId, RepositoryAccessLevel type)
        {
            var repository = FindRepository(repositoryId, RepositoryAccessLevel.EDIT, HttpContext.User.Identity);
            if (repository == null) return NotFound();
            throw new NotImplementedException();
        }
        #endregion
        //Requests regarding the version tree, and specific versions in particular.
        #region Versions
        [HttpGet]
        public ActionResult VersionDetails(Guid repositoryId, Guid versionId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult CreateVersion([FromForm] VersionCreateModel data)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult DeleteVersion(Guid repositoryId, Guid versionId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
