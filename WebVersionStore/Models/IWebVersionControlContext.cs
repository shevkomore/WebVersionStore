using Microsoft.EntityFrameworkCore;

namespace WebVersionStore.Models
{
    public interface IWebVersionControlContext
    {
        public DbSet<Repository> Repositories { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserRepositoryAccess> UserRepositoryAccesses { get; set; }

        public DbSet<Version> Versions { get; set; }
    }
}
