namespace WebVersionStore.Models
{
    public class ResponceModel
    {
        string Status { get; set; }
        object? Responce { get; set; }
        public ResponceModel(string status, object Responce)
        {
            this.Status = status;
            this.Responce = Responce;
        }
        public ResponceModel(object Responce) 
            :this("Success", Responce) { }

    }
}
