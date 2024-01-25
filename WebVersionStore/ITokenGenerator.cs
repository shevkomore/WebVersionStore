namespace WebVersionStore
{
    public interface ITokenGenerator
    {
        public string GenerateUserToken(string userIdentity);
    }
}
