using Microsoft.EntityFrameworkCore;

namespace WebVersionStore.Models.Database
{
    public partial class WebVersionControlContext
    {
        [DbFunction(Name = "dbo.UserAccessLevel", IsBuiltIn = true, IsNullable = true)]
        public static UserAccessLevelModel GetAccessLevel(string user, Guid repo)
        {
            throw new NotImplementedException();
        }
    }
}
