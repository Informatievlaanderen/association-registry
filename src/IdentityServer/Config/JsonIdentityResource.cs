namespace IdentityServer.Config;

using Duende.IdentityServer.Models;

public class JsonIdentityResource
{
    private static readonly Dictionary<string, IdentityResource> Predefined =
        new()
        {
            { "identityresources.openid", new IdentityResources.OpenId() },
            { "identityresources.profile", new IdentityResources.Profile() },
        };

    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public List<string>? UserClaims { get; set; }

    public static IdentityResource Export(JsonIdentityResource resource)
        => Predefined.ContainsKey(resource.Name.ToLower())
            ? Predefined[resource.Name.ToLower()]
            : new IdentityResource(resource.Name, resource.DisplayName, resource.UserClaims);
}
