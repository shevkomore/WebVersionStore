using WebVersionStore.Models.Database;

namespace WebVersionStore.Models
{
    public class RepositoryAccessSettingsModel
    {
        public bool CanView { get; set; }

        public bool CanEdit { get; set; }

        public bool CanAdd { get; set; }

        public bool CanRemove { get; set; }

        public bool IsOwner { get; set; }

        public RepositoryAccessSettingsModel() { }

        public RepositoryAccessSettingsModel(UserRepositoryAccess from, Repository? from2 = null)
        {
            if(from2 != null && from2.Author == from.UserLogin) 
            {
                //User has created the database, which automatically grants them the maximum access level
                IsOwner = true;
                CanAdd = true;
                CanRemove = true;
                CanView = true;
                CanEdit = true;
                return;
            }
            CanView = from.CanView;
            CanEdit = from.CanEdit;
            CanAdd = from.CanAdd;
            CanRemove = from.CanRemove;

        }
        public 
    }
}
