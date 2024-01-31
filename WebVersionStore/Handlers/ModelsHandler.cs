using System.Linq.Expressions;
using System.Security.Principal;
using WebVersionStore.Models;
using WebVersionStore.Models.Database;

namespace WebVersionStore.Handlers
{
    public static class ModelsHandler
    {
        #region RepositoryAccessLevel
        /*Cannot be used in LINQ expressions!*/
        public static Dictionary<RepositoryAccessLevel, Func<UserAccessLevelResult, bool>> AccessChecks = new Dictionary<RepositoryAccessLevel, Func<UserAccessLevelResult, bool>>
        {
            { RepositoryAccessLevel.VIEW, o => o.CanView??false },
            { RepositoryAccessLevel.ADD, o => o.CanAdd??false },
            { RepositoryAccessLevel.EDIT, o => o.CanEdit??false },
            { RepositoryAccessLevel.REMOVE, o => o.CanRemove??false },
            { RepositoryAccessLevel.AUTHOR, o => o.IsAuthor??false },
        };
        public static bool Check(this RepositoryAccessLevel required, UserAccessLevelResult to_check) 
            => AccessChecks[required](to_check);
        public static async void Grant(this RepositoryAccessLevel level, string user, Guid repositoryId, WebVersionControlContext db)
        {
            var access = await FindOrCreateRepositoryAccess(user, repositoryId, db);
            //If any access is granted, VIEW access is granted by default
            access.CanView = true;
            switch (level)
            {
                case RepositoryAccessLevel.EDIT:
                    access.CanEdit = true; break;
                case RepositoryAccessLevel.ADD:
                    access.CanAdd = true; break;
                case RepositoryAccessLevel.REMOVE:
                    access.CanRemove = true; break;
            }
        }
        public static async void Revoke(this RepositoryAccessLevel level, string user, Guid repositoryId, WebVersionControlContext db)
        {
            var access = await FindOrCreateRepositoryAccess(user, repositoryId, db);
            switch (level)
            {
                case RepositoryAccessLevel.VIEW:
                    //If VIEW access is revoked, all other access is revoked as well
                    access.CanView = false;
                    access.CanEdit = false;
                    access.CanAdd = false;
                    access.CanRemove = false;
                    break;
                case RepositoryAccessLevel.EDIT:
                    access.CanEdit = false; break;
                case RepositoryAccessLevel.ADD:
                    access.CanAdd = false; break;
                case RepositoryAccessLevel.REMOVE:
                    access.CanRemove = false; break;
            }
        }
        public static async Task<UserRepositoryAccess> FindOrCreateRepositoryAccess(string user, Guid repositoryId, WebVersionControlContext db)
        {
            var res = await db.UserRepositoryAccesses.FindAsync(user, repositoryId);
            if (res == null)
            {
                res = new UserRepositoryAccess
                {
                    UserLogin = user,
                    RepositoryId = repositoryId
                };
                db.UserRepositoryAccesses.Add(res);
            }
            return res;
        }
        #endregion
        /*public static bool CanAccess(this IIdentity user, Repository repository, RepositoryAccessLevel requirement)
        {
            return user != null
                && (
                repository.Author == user.Name
                || repository.UserRepositoryAccesses.Any(access =>
                    access.UserLogin == user.Name
                    && requirement.Check(access)
                   )
                );
        }*/
        public static bool CanAccess(this IIdentity user, Guid repositoryId, WebVersionControlContext db, RepositoryAccessLevel requirement)
        {
            return user != null
                && db.UserAccessLevel(user.Name, repositoryId).Any(access => 
                    access.UserId == user.Name
                  && (!(requirement == RepositoryAccessLevel.VIEW)  || (access.CanView??false))
                  && (!(requirement == RepositoryAccessLevel.EDIT)  || (access.CanEdit ?? false))
                  && (!(requirement == RepositoryAccessLevel.ADD)   || (access.CanAdd ?? false))
                  && (!(requirement == RepositoryAccessLevel.REMOVE)|| (access.CanRemove ?? false))
                  && (!(requirement == RepositoryAccessLevel.AUTHOR) || (access.IsAuthor ?? false))
                  );
                
        }
        public static Repository Apply(this RepositoryDisplaySettingsModel model, Repository repository)
        {
            repository.Name = model.Name;
            repository.Description = model.Description;
            return repository;
        }
    }
}
