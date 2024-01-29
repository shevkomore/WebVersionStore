using WebVersionStore.Models.Database;
using Version = WebVersionStore.Models.Database.Version;

namespace WebVersionStore.Models
{
    public class VersionCreateModel
    {
        public Guid Repository { get; set; }
        public Guid? Parent { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Color { get; set; }
        public IFormFile Data { get; set; }
        public IFormFile? Image { get; set; }
        public Version BuildVersion(string? imageLocation, string dataLocation)
        {
            return new Version
            {
                RepositoryId = Repository,
                Name = Name,
                Description = Description,
                Color = Color,
                ImageLocation = imageLocation,
                DataLocation = dataLocation
            };
        }
    }
}
