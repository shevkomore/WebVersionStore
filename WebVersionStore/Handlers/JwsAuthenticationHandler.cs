using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;

namespace WebVersionStore.Handlers
{
    public class JwsAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public JwsAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            :base(options, logger, encoder, clock)
        { }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("jwt"))
                return AuthenticateResult.Fail("Missing key");

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["jwt"]);
            var authHeaderData = 
                Encoding.UTF8.GetString(
                Convert.FromBase64String(authHeader.Parameter)
                );

            return AuthenticateResult.Fail("Cannot process");
        }
    }
}
