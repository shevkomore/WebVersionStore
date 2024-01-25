using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebVersionStore.Handlers
{
    public static class JwtTokenValidationHandler
    {
        public static TokenValidatedContext ValidateJwtToken(this TokenValidatedContext context)
        {
            var login = context.Principal.Identity.Name;
            if (login == null)//TODO check login & password
                context.Fail("Unauthorized");
            return context;
        }
    }
}
