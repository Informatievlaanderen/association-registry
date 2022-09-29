namespace IdentityServer.Config;

using Duende.IdentityServer.Models;

public class JsonApiResource
{
    public string Name { get; set; } = null!;
    public List<string> ApiSecrets { get; set; } = new();
    public List<string> Scopes { get; set; } = new();

    public static ApiResource Export(JsonApiResource resource)
        => new(resource.Name)
        {
            ApiSecrets = resource.ApiSecrets
                .Select(
                    apiSecret =>
                        new Secret(apiSecret.Sha256()))
                .ToList(),
            Scopes = resource.Scopes,
        };
}
