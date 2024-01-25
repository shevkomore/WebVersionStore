using Microsoft.EntityFrameworkCore;

namespace WVS_DAL.EntityFramework
{
    public class EntityDatabase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
