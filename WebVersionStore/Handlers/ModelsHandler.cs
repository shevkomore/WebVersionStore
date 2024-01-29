using System.Security.Principal;
using WebVersionStore.Models;
using WebVersionStore.Models.Database;

namespace WebVersionStore.Handlers
{
    public static class ModelsHandler
    {
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

        public static bool CanAccess(this IIdentity user, Repository repository, RepositoryAccessLevel requirement)
        {
            return user != null
                && (
                repository.Author == user.Name
                || repository.UserRepositoryAccesses.Any(access =>
                    access.UserLogin == user.Name
                    && requirement.Check(access)
                   )
                );
        }
    }
}
