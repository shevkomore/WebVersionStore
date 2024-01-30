namespace WebVersionStore.Models.Database
{
    public class UserAccessLevelModel
    {
        public Guid? RepositoryId { get; set; }
        public string? UserId { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? CanView { get; set; }
        public bool? CanAdd { get; set; }
        public bool? CanRemove { get; set; }
        public bool? CanEdit { get; set; }
    }
}
