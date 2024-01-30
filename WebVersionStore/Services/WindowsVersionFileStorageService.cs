using System.Drawing;
using System.IO;
using WebVersionStore.Models.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebVersionStore.Services
{
    public class WindowsVersionFileStorageService : IVersionFileStorageService
    {
        static readonly string DATA_FOLDER = "Data";
        static readonly string IMAGES_FOLDER = "Images";
        public string StoreData(Guid repository, Stream data)
        {
            var fileLocation = Guid.NewGuid().ToString();
            CreateRequiredFolders(repository);
            using (FileStream file = System.IO.File.Create(
                BuildDataFilePath(repository, fileLocation)
                ))
            {
                data.CopyTo(file);
            }
            return fileLocation;
        }
        public string StoreData(Guid repository, IFormFile file)
            => StoreData(repository, file.OpenReadStream());

        public string StoreImage(Guid repository, Stream image)
        {
            var imageSource = Image.FromStream(image);
            var resizedImage = new Bitmap(imageSource, new Size(300, 200));
            var imageLocation = Guid.NewGuid().ToString();
            CreateRequiredFolders(repository);
            resizedImage.Save(
                BuildImageFilePath(repository, imageLocation)
                );
            return imageLocation;
        }
        public string StoreImage(Guid repository, IFormFile file)
            => StoreImage(repository, file.OpenReadStream());


        public bool DeleteData(Guid repository, string dataLocation)
            => Delete(BuildDataFilePath(repository, dataLocation));
        public bool DeleteImage(Guid repository, string imageLocation)
            => Delete(BuildImageFilePath(repository, imageLocation));

        public string GetDataPath(Guid repository, string dataLocation)
        {
            return BuildDataFilePath(repository, dataLocation);
        }
        public string? GetImagePath(Guid repository, string imageLocation)
        {
            return BuildImageFilePath(repository, imageLocation);
        }
        



        static string BuildDataFilePath(Guid repositoryId, string fileName)
        {
            return Path.Combine(
                BuildDirectoryPath(repositoryId, DATA_FOLDER),
                fileName
                );
        }
        static string BuildImageFilePath(Guid repositoryId, string fileName)
        {
            return Path.Combine(
                BuildDirectoryPath(repositoryId, IMAGES_FOLDER),
                fileName + ".png"
                );
        }
        static string BuildDirectoryPath(Guid repositoryId, string folder)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "WebVersionStore",
                folder,
                repositoryId.ToString());
        }
        static void CreateRequiredFolders(Guid repositoryId)
        {
            Directory.CreateDirectory(Path.Combine(BuildDirectoryPath(repositoryId, DATA_FOLDER)));
            Directory.CreateDirectory(Path.Combine(BuildDirectoryPath(repositoryId, IMAGES_FOLDER)));
        }
        static bool Delete(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
