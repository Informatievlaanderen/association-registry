namespace IdentityServer.Config;

using Duende.IdentityServer.Models;

public class JsonConfig
{
    public List<JsonIdentityResource> IdentityResources { get; set; } = new();
    public List<string> ApiScopes { get; set; } = new();
    public List<JsonApiResource> ApiResources { get; set; } = new();
    public List<JsonClient> Clients { get; set; } = new();

    public IEnumerable<IdentityResource> GetIdentityResources()
        => IdentityResources.Select(JsonIdentityResource.Export);

    public IEnumerable<ApiScope> GetApiScopes()
        => ApiScopes.Select(s => new ApiScope(s));

    public IEnumerable<ApiResource> GetApiResources()
        => ApiResources.Select(JsonApiResource.Export);

    public IEnumerable<Client> GetClients()
        => Clients.Select(JsonClient.Export);

    public static JsonConfig Merge(JsonConfig jc1, JsonConfig jc2)
        => new()
        {
            IdentityResources = jc1.IdentityResources.MergeLists(jc2.IdentityResources, areSame: (r1, r2) => r1.Name == r2.Name),
            ApiScopes = jc1.ApiScopes.MergeLists(jc2.ApiScopes, areSame: (s1, s2) => s1 == s2),
            ApiResources = MergeApiResources(jc1.ApiResources, jc2.ApiResources).ToList(),
            Clients = jc1.Clients.MergeLists(jc2.Clients, areSame: (c1, c2) => c1.ClientId == c2.ClientId).ToList(),
        };

    private static List<JsonApiResource> MergeApiResources(List<JsonApiResource> list1, List<JsonApiResource> list2)
    {
        var result = list1;

        foreach (var item in list2)
        {
            if (result.Any(x => x.Name == item.Name))
            {
                var element = result.Single(x => x.Name == item.Name);
                element.Scopes = element.Scopes.MergeLists(item.Scopes, (s1, s2) => s1 == s2);
                element.ApiSecrets = element.ApiSecrets.MergeLists(item.ApiSecrets, (s1, s2) => s1 == s2);
            }
            else
            {
                result.Add(item);
            }
        }

        return result;
    }
}
