using System.Runtime.ConstrainedExecution;
using WebVersionStore.Models.Database;

namespace WebVersionStore.Models
{
    public class RepositoryResponceModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public RepositoryAccessSettingsModel Access { get; set; }
        public List<VersionResponceModel> Versions { get; set; }
        public RepositoryResponceModel
            (Repository repository, List<VersionResponceModel> versions, 
            RepositoryAccessSettingsModel access) 
        {
            Id = repository.RepositoryId;
            Name = repository.Name;
            Description = repository.Description;
            Versions = versions;
            Access = access;
        }
    }
}
