using WebVersionStore.Models.Database;

namespace WebVersionStore.Models
{
    public class RepositoryListResponceItemModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public RepositoryListResponceItemModel(Repository repo) 
        {
            Id = repo.RepositoryId;
            Name = repo.Name;
            Author = repo.Author;
        }
    }
}
