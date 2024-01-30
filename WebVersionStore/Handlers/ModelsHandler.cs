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
        public static Dictionary<RepositoryAccessLevel, Func<UserRepositoryAccess, bool>> AccessChecks = new Dictionary<RepositoryAccessLevel, Func<UserRepositoryAccess, bool>>
        {
            { RepositoryAccessLevel.VIEW, o => o.CanView },
            { RepositoryAccessLevel.ADD, o => o.CanAdd },
            { RepositoryAccessLevel.EDIT, o => o.CanEdit },
            { RepositoryAccessLevel.REMOVE, o => o.CanRemove },
            { RepositoryAccessLevel.AUTHOR, o => false },
        };
        public static bool Check(this RepositoryAccessLevel required, UserRepositoryAccess to_check) 
            => AccessChecks[required](to_check);
        public static void Grant(this RepositoryAccessLevel level, UserRepositoryAccess access)
        {
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
        public static void Revoke(this RepositoryAccessLevel level, UserRepositoryAccess access)
        {
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
        public static bool CanAccess(this IIdentity user, Repository repository, RepositoryAccessLevel requirement)
        {
            return user != null
                && ( repository.Author == user.Name
                || repository.UserRepositoryAccesses.Any(access => 
                    access.UserLogin == user.Name
                  && (!(requirement == RepositoryAccessLevel.VIEW)  || access.CanView)
                  && (!(requirement == RepositoryAccessLevel.EDIT)  || access.CanEdit)
                  && (!(requirement == RepositoryAccessLevel.ADD)   || access.CanAdd)
                  && (!(requirement == RepositoryAccessLevel.REMOVE)|| access.CanRemove)
                  ));
                
        }
        public static Repository Apply(this RepositoryDisplaySettingsModel model, Repository repository)
        {
            repository.Name = model.Name;
            repository.Description = model.Description;
            return repository;
        }
    }
}
