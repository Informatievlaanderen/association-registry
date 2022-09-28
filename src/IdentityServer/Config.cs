using Duende.IdentityServer.Models;

namespace IdentityServer;

using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Test;
using IdentityModel;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources
        => new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new("vo", "Vlaamse Overheid", new[] { "vo_info" }),
            new("dv", "Digitaal Vlaanderen", new[] { "dv_verenigingsregister_hoofdvertegenwoordigers" }),
        };

    public static IEnumerable<ApiScope> ApiScopes
        => new ApiScope[]
        {
            new("vo_info"),
            new("dv_verenigingsregister_hoofdvertegenwoordigers"),
        };

    public static IEnumerable<ApiResource> ApiResources
        => new[]
        {
            new ApiResource("association-registry-local-dev")
            {
                ApiSecrets = new List<Secret>()
                {
                    new("a_very=Secr3t*Key".Sha256())
                },
                Scopes = new List<string>()
                {
                    "vo_info",
                    "dv_verenigingsregister_hoofdvertegenwoordigers",
                }
            }
        };

    public static IEnumerable<Client> Clients
        => new[]
        {
            new Client
            {
                ClientId = "association-registry-local-dev",
                ClientSecrets =
                {
                    new Secret(
                        "a_very=Secr3t*Key".Sha256())
                },

                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "vo_info",
                },
                AlwaysSendClientClaims = true,
                AlwaysIncludeUserClaimsInIdToken = true,
            },
            new Client
            {
                ClientId = "acmClient",
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "dv_verenigingsregister_hoofdvertegenwoordigers" },
                AccessTokenLifetime = int.MaxValue,
                IdentityTokenLifetime = int.MaxValue,

                ClientClaimsPrefix = string.Empty,
                // scopes that client has access to
                Claims = new List<ClientClaim>()
                {
                    //new ClientClaim("dv_organisatieregister_orgcode", "OVO000001")
                }
            },
        };

    public static List<TestUser> Users
        => new()
        {
            new TestUser
            {
                Username = "dev",
                Password = "dev",
                IsActive = true,
                Claims = new List<Claim>
                {
                    new("vo_id", "9C2F7372-7112-49DC-9771-F127B048B4C7"),
                    new(JwtClaimTypes.FamilyName, "Persona"),
                    new(JwtClaimTypes.GivenName, "Developer"),
                    new("iv_verenigingsregister_rol_3D", "Dienstverleningsregister-admin"),
                },
                SubjectId = "dev",
            },
            new TestUser
            {
                Username = "acm",
                Password = "acm",
                IsActive = true,
                Claims = new List<Claim>
                {
                    new("vo_id", "E6D110DC-231A-4666-BAFB-C354255EF547"),
                    new(JwtClaimTypes.FamilyName, "Persona"),
                    new(JwtClaimTypes.GivenName, "Vlimpers"),
                },
                SubjectId = "acm",
            },
        };
}
