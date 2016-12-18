using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Configuration;
using System;

[assembly: OwinStartup(typeof(RoboBank.BankingPortal.MVC.Startup))]

namespace RoboBank.BankingPortal.MVC
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                ExpireTimeSpan = TimeSpan.FromDays(60)
            });


            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = ConfigurationManager.AppSettings["OpenIdConnectAuthority"],
                ClientId = ConfigurationManager.AppSettings["AppID"],
                RedirectUri = ConfigurationManager.AppSettings["RedirectUri"],
                PostLogoutRedirectUri = ConfigurationManager.AppSettings["RedirectUri"],
                SignInAsAuthenticationType = "Cookies",
                //This is needed in order to NOT set the expiration of the OIDC id_token on AuthenticationTicket.
                // For true (the default), the OIDC id_token expiration will be applied on the AuthenticationTicket
                // which is later (on the response to OIDC request after the redirection from Id Server) picked up by the CookieAuthenticationMiddleware and applied on the internal expiration 
                // field that is encoded in the value of ".AspNetCookies" cookie overriding any value you might set on the CookieAuthenticationOptions
                UseTokenLifetime = false,

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = n =>
                    {
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        return Task.FromResult(0);
                    },

                    RedirectToIdentityProvider = n =>
                        {
                            if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                            {
                                var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                                if (idTokenHint != null)
                                {
                                    n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                                }
                            }

                            return Task.FromResult(0);
                        }
                }
            });
        }
    }
}