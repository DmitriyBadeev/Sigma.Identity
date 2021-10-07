using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace Sigma.Identity.Web
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
        public static IEnumerable<ApiResource> GetApiResource()
        {
            return new List<ApiResource>
            {
                new ApiResource("Sigma.Api")
                {
                    Scopes = { "Sigma.Api.All" }
                },
                new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
            };
        }

        public static IEnumerable<ApiScope> GetApiScope()
        {
            return new List<ApiScope>
            {
                new ApiScope("Sigma.Api.All"),
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
            };
        }
        
        public static IEnumerable<Client> GetSpaClient()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "Sigma.Spa",
                    ClientName = "Sigma",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AccessTokenLifetime = 3600 * 24,
                    RequireConsent = false,
                    AllowedCorsOrigins = 
                    { 
                        "http://localhost:3000", 
                        "http://localhost:3001",
                        "https://sigma-frontend.herokuapp.com"
                    },
                    PostLogoutRedirectUris = new List<string> 
                    {
                        "http://localhost:3000/signout",
                        "https://sigma-frontend.herokuapp.com/signout"
                    },
                    RedirectUris = new List<string>
                    {
                        "http://localhost:3000/auth-complete",
                        "https://sigma-frontend.herokuapp.com/auth-complete"
                    },
                    AllowedScopes = 
                    { 
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.LocalApi.ScopeName,
                        "Sigma.Api"
                    }
                }
            };
        }
    }
}