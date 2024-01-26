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
        //Non-specific requests, and non-action methods (private/with NonActionAttribute)
        #region Generic
        [HttpGet]
        public ActionResult Index() => RedirectToActionPermanent(nameof(List));

        Repository? FindRepository(Guid repositoryId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult ListAccess(RepositoryAccessSettingsModel accessSettings)
        {
            //show ones with this or higher access
            throw new NotImplementedException();
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
            var repository = FindRepository(repositoryId);
            if (repository == null) return NotFound();
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult GrantAccess(Guid repositoryId, RepositoryAccessLevel type) 
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult RevokeAccess(Guid repositoryId, RepositoryAccessLevel type)
        {
            throw new NotImplementedException();
        }
        //TODO Stopped here. This region' actions are probably done.
        #endregion
        //Requests regarding the version tree, and specific versions in particular.
        #region Versions

        #endregion
    }
}
