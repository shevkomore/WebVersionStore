namespace WebVersionStore.Services
{
    public interface IVersionFileStorageService
    {
        public string StoreImage(Guid repository, Stream file);
        public string StoreImage(Guid repository, IFormFile file);
        public string StoreData(Guid repository, Stream file);
        public string StoreData(Guid repository, IFormFile file);
        public string? GetImagePath(Guid repository, string imageLocation);
        public string GetDataPath(Guid repository, string dataLocation);
        public bool DeleteImage(Guid repository, string imageLocation);
        public bool DeleteData(Guid repository, string dataLocation);
    }
}
