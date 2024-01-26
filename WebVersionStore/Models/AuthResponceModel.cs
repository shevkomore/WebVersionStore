namespace WebVersionStore.Models
{
    public class AuthResponceModel
    {
        public string Login { get; set; }
        public string Token { get; set; }

        public AuthResponceModel(AuthRequestModel req)
        {
            this.Login = req.Login;
        }
        public AuthResponceModel() { }
    }
}
