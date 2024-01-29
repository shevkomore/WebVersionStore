﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Principal;
using WebVersionStore.Handlers;
using WebVersionStore.Models;
using WebVersionStore.Models.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebVersionStore.Controllers
{
    [Authorize]
    public class RepositoryController : Controller
    {
        WebVersionControlContext _database;
        IConfiguration _configuration;
        public RepositoryController(WebVersionControlContext database, IConfiguration config)
        {
            _database = database;
            _configuration = config;
        }
        //Non-specific requests, and non-action methods (private/with NonActionAttribute)
        #region General
        [HttpGet]
        public ActionResult Index() => RedirectToActionPermanent(nameof(List));

        ValueTask<Repository?> FindRepository(Guid repositoryId)
        {
            return _database.Repositories.FindAsync(repositoryId);
        }
        async Task<Repository?> FindRepository(Guid repositoryId, RepositoryAccessLevel level, IIdentity? user)
        {
            if (user == null) return null;
            var repository = await FindRepository(repositoryId);
            if (repository == null) return null;
            if (!user.CanAccess(repository, level)) return null;
            return repository;
        }
        string BuildDataFilePath(Guid repositoryId, string fileName)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                repositoryId.ToString(),
                "Data",
                fileName
                );
        }
        string BuildImageFilePath(Guid repositoryId, string fileName)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                repositoryId.ToString(),
                "Images",
                fileName + ".png"
                );
        }
        #endregion
        //Requests regarding the list of all repositories available to user
        #region RepositoriesList
        [HttpGet]
        public ActionResult List()
        {
            if (HttpContext.User.Identity?.Name == null)
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
            if (HttpContext.User.Identity?.Name == null)
                return BadRequest();

            var list = from repo in _database.Repositories
                       where repo.Author == HttpContext.User.Identity.Name
                       select repo;
            return Json(list.ToList());
        }

        [HttpGet]
        public ActionResult ListAccess(RepositoryAccessSettingsModel accessSettings)
        {
            if (HttpContext.User.Identity?.Name == null)
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
        //Requests regarding a specific repository. Usually call FindRepository (in General)
        #region Repository
        [HttpGet]
        public async Task<ActionResult> Details(Guid repositoryId)
        {
            var repository = await FindRepository(repositoryId, RepositoryAccessLevel.VIEW, HttpContext.User.Identity);
            if (repository == null) return NotFound();
            //The version tree is also sent here
            //TODO? prepare tree data here for simpler visualization (sort by heredity, maybe construct the tree etc.)
            var tree = repository.Versions.ToList();
            return Json(new {info = repository, tree = tree });
        }

        [HttpPost]
        public async Task<ActionResult> Create(RepositoryDisplaySettingsModel model)
        {
            if (HttpContext.User.Identity?.Name == null)
                return BadRequest();
            if (_database.Users.Find(HttpContext.User.Identity) == null)
                return NotFound("User not found");

            _database.Repositories.Add(new Repository
            {
                Author = HttpContext.User.Identity.Name!,
                Name = model.Name,
                Description = model.Description,
            });

            if (await _database.SaveChangesAsync() == 0)
                return StatusCode(500);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Guid repositoryId, RepositoryDisplaySettingsModel model)
        {
            var repository = await FindRepository(repositoryId, RepositoryAccessLevel.EDIT, HttpContext.User.Identity);
            if (repository == null) return NotFound();

            model.Apply(repository);

            await _database.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> GrantAccess(Guid repositoryId, string target, RepositoryAccessLevel level) 
        {
            /* Explanation:
             *  There are three considered access levels:
             *      Author
             *        Edit-level users
             *          Other users
             *  Other users cannot change access
             *  Each given level can grant access UP TO their level 
             *      (NOT inclusive; i.e. Edit-level cannot grant CanEdit access)
             *  User cannot grant access to themselves 
             *      (it's generally unnecessary; it's checked here just in case something else made a mistake)
             */
            var repository = await FindRepository(repositoryId, RepositoryAccessLevel.EDIT, HttpContext.User.Identity);
            if (repository == null) return NotFound();

            if (target == HttpContext.User.Identity?.Name)
                return BadRequest("User cannot grant access to themselves");
            if (target == repository.Author)
                return Ok("Author does not require access level assignment");
            if (level == RepositoryAccessLevel.EDIT)
            {
                var source = await _database.Users.FindAsync(HttpContext.User.Identity);
                if (source == null || repository.Author != source.Login)
                    return Unauthorized("At least Author access level is required to grant EDIT access");
            }
            var targetAccess = await _database.UserRepositoryAccesses.FindAsync(repositoryId, target);
            if (targetAccess == null) targetAccess = new UserRepositoryAccess{
                    RepositoryId = repositoryId,
                    UserLogin = target
                };
            level.Grant(targetAccess);
            _database.UserRepositoryAccesses.Add(targetAccess);
            await _database.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> RevokeAccess(Guid repositoryId, string target, RepositoryAccessLevel level)
        {
            /* Explanation:
             * There are three considered access levels:
             *      Author
             *        Edit-level users
             *          Other users
             *  Other users can only revoke their own access
             *  Each given level can revoke access BELOW their level 
             *      (Edit-level cannot revoke access of other Edit-level users)
             *  Author cannot lose access
             */
            var source = await _database.Users.FindAsync(HttpContext.User.Identity);
            if (source == null) return Unauthorized();

            var repository = await FindRepository(repositoryId);
            if (repository == null) return NotFound();

            if (target == repository.Author)
                return BadRequest("Author cannot lose access");

            var targetAccess = await _database.UserRepositoryAccesses.FindAsync(repositoryId, target);
            if (targetAccess == null || !level.Check(targetAccess)) 
                return Ok("User already has insufficient access");

            if (!(source.Login == repository.Author || targetAccess.CanEdit)
                && target != source.Login)
                return Unauthorized("Without EDIT access only user's own access can be revoked");

            if (source.Login != repository.Author && targetAccess.CanEdit)
                return Unauthorized("Only Author can revoke EDIT access");

            level.Revoke(targetAccess);
            await _database.SaveChangesAsync();
            return Ok();
        }
        #endregion
        //Requests regarding the version tree, and specific versions in particular.
        #region Versions
        [HttpGet]
        public async Task<ActionResult> VersionDetails(Guid repositoryId, Guid versionId)
        {

            //var repository = await FindRepository(repositoryId, RepositoryAccessLevel.VIEW, HttpContext.User.Identity);
            //if (repository == null) return NotFound();
            if (!_database.UserRepositoryAccesses.Any(access =>
                    access.RepositoryId == repositoryId
                    && (access.UserLogin == HttpContext.User.Identity.Name)
                    && RepositoryAccessLevel.VIEW.Check(access)))
                return Unauthorized();

            var version = await _database.Versions.FindAsync(versionId);
            if (version == null || version.RepositoryId != repositoryId) return NotFound();

            //TODO? also send parent and children
            return Json(new VersionResponceModel(version));
        }

        [HttpPost]
        public async Task<ActionResult> CreateVersion([FromForm] VersionCreateModel data)
        {
            var repository = await FindRepository(data.Repository, RepositoryAccessLevel.ADD, HttpContext.User.Identity);
            if (repository == null) return NotFound();

            string? imageLocation = null;
            if (data.Image != null)
            {
                var imageSource = Image.FromStream(data.Image.OpenReadStream());
                var resizedImage = new Bitmap(imageSource, new Size(300, 200));
                imageLocation = Guid.NewGuid().ToString();
                resizedImage.Save(
                    BuildImageFilePath(data.Repository, imageLocation)
                    );
            }
            var fileLocation = Guid.NewGuid().ToString();
            using(FileStream file = System.IO.File.Create(
                BuildImageFilePath(data.Repository, fileLocation)
                ))
            {
                data.Data.CopyTo(file);
            }
            var version = data.BuildVersion(imageLocation, fileLocation);
            //!!!!!THIS MIGHT NOT WORK (might not be sent to database)
            repository.Versions.Add(version);
            await _database.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> RemoveVersion(Guid repositoryId, Guid versionId)
        {
            var repository = await FindRepository(repositoryId, RepositoryAccessLevel.REMOVE, HttpContext.User.Identity);
            if (repository == null) return NotFound();

            var version = await _database.Versions.FindAsync(versionId);
            if (version == null || version.RepositoryId != repositoryId) return NotFound();

            System.IO.File.Delete(BuildDataFilePath(repository.RepositoryId, version.DataLocation));
            if(version.ImageLocation != null)
                System.IO.File.Delete(BuildImageFilePath(repository.RepositoryId, version.ImageLocation));

            _database.Versions.Remove(version);
            await _database.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> LoadVersion(Guid repositoryId, Guid versionId)
        {
            var repository = await FindRepository(repositoryId, RepositoryAccessLevel.REMOVE, HttpContext.User.Identity);
            if (repository == null) return NotFound();

            var version = await _database.Versions.FindAsync(versionId);
            if (version == null || version.RepositoryId != repositoryId) return NotFound();

            await Response.SendFileAsync(BuildDataFilePath(repository.RepositoryId, version.DataLocation));
            return Ok();
        }
        #endregion
    }
}
