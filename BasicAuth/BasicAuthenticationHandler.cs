using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//source: https://www.roundthecode.com/dotnet/how-to-add-basic-authentication-to-asp-net-core-application

namespace BasicAuth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            ) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Response.Headers.Add("WWW-Authenticate", "Basic");

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header missing."));
            }

            // извлекаем логин и пароль
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var authHeaderRegex = new Regex(@"Basic (.*)");

            if (!authHeaderRegex.IsMatch(authorizationHeader))
            {
                return Task
                    .FromResult(AuthenticateResult
                                    .Fail("Authorization code not formatted properly."));
            }

            var authBase64 = Encoding.UTF8.GetString(Convert
                .FromBase64String(authHeaderRegex.Replace(authorizationHeader, "$1")));
            var authSplit = authBase64.Split(':', 2);
            var authUsername = authSplit[0];
            var authPassword = authSplit.Length > 1 
                ? authSplit[1] 
                : throw new Exception("Unable to get password");

            // тут должна быть проверка данных из БД пользователей
            if (authUsername != "roundthecode" || authPassword != "roundthecode")
            {
                return Task.FromResult(AuthenticateResult
                    .Fail("The username or password is not correct."));
            }

            var authenticatedUser = new AuthenticatedUser("BasicAuthentication", true, "roundthecode");
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser));

            return Task.FromResult(AuthenticateResult
                .Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        }
    }
}
