using WebVersionStore.Services;
using Version = WebVersionStore.Models.Database.Version;

namespace WebVersionStore.Models
{
    public class VersionEditModel
    {
        public Guid RepositoryId { get; set; }

        public Guid VersionId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Color { get; set; }

        public IFormFile? Image { get; set; }

        public Version Update(Version version, IVersionFileStorageService _files)
        {
            if(Name != null) version.Name = Name;
            if(Description != null) version.Description = Description;
            if(Color != null) version.Color = Color;
            if(Image != null)
            {
                if(version.ImageLocation != null)
                    _files.DeleteImage(RepositoryId, version.ImageLocation);
                version.ImageLocation = _files.StoreImage(VersionId, Image);
            }
            return version;
        }
    }
}
