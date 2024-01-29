using WebVersionStore.Models.Database;
using Version = WebVersionStore.Models.Database.Version;

namespace WebVersionStore.Models
{
    public class VersionResponceModel
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Color { get; set; } = null!;

        public string? ImageLocation { get; set; }
        public VersionResponceModel(Version version)
        {
            Name = version.Name;
            Description = version.Description;
            Color = version.Color;
            ImageLocation = version.ImageLocation;
        }
    }
}
