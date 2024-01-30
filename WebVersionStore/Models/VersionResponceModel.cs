using WebVersionStore.Models.Database;
using WebVersionStore.Services;
using Version = WebVersionStore.Models.Database.Version;

namespace WebVersionStore.Models
{
    public class VersionResponceModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Color { get; set; } = null!;

        public string? ImageLocation { get; set; }
        public VersionResponceModel(Version version, IVersionFileStorageService _file)
        {
            Id = version.VersionId;
            Name = version.Name;
            Description = version.Description;
            Color = version.Color;
            if(version.ImageLocation != null)
                ImageLocation = _file.GetImagePath(version.RepositoryId, version.ImageLocation);
        }
    }
}
