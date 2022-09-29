namespace IdentityServer;

using Duende.IdentityServer;
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

public class JsonClient
{
    public string ClientId { get; set; } = null!;
    public List<string> ClientSecrets { get; set; } = new();
    public string AllowedGrantTypes { get; set; } = null!;
    public List<string> AllowedScopes { get; set; } = new();
    public bool? AlwaysSendClientClaims { get; set; }
    public bool? AlwaysIncludeUserClaimsInIdToken { get; set; }
    public int? AccessTokenLifetime { get; set; }
    public int? IdentityTokenLifetime { get; set; }
    public string? ClientClaimsPrefix { get; set; }

    public static Client Export(JsonClient jsonClient)
    {
        var client = new Client
        {
            ClientId = jsonClient.ClientId,
            ClientSecrets = jsonClient.ClientSecrets
                .Select(
                    secret =>
                        new Secret(secret.Sha256()))
                .ToList(),
            AllowedGrantTypes = GetAllowedGrantTypes(jsonClient.AllowedGrantTypes),
            AllowedScopes = GetAllowedScopes(jsonClient.AllowedScopes),
        };

        client.SetAccessTokenLifetimeOrDefault(jsonClient.AccessTokenLifetime);
        client.SetIdentityTokenLifetimeOrDefault(jsonClient.IdentityTokenLifetime);
        client.SetClientClaimsPrefixOrDefault(jsonClient.ClientClaimsPrefix);
        client.SetAlwaysSendClientClaimsOrDefault(jsonClient.AlwaysSendClientClaims);
        client.SetAlwaysIncludeUserClaimsInIdTokenOrDefault(jsonClient.AlwaysIncludeUserClaimsInIdToken);

        return client;
    }

    private static ICollection<string> GetAllowedScopes(List<string> allowedScopes)
    {
        Dictionary<string, string> predefinedScopes =
            new()
            {
                { "standardscopes.openid", IdentityServerConstants.StandardScopes.OpenId },
                { "standardscopes.profile", IdentityServerConstants.StandardScopes.Profile },
            };

        return allowedScopes
            .Select(scope => predefinedScopes.ContainsKey(scope.ToLower()) ? predefinedScopes[scope.ToLower()] : scope)
            .ToList();
    }

    private static ICollection<string> GetAllowedGrantTypes(string allowedGrantTypes)
        => allowedGrantTypes.ToLowerInvariant() switch
        {
            "clientcredentials" => GrantTypes.ClientCredentials,
            "code" => GrantTypes.Code,
            _ => throw new NotSupportedException(),
        };
}

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

public static class Extensions
{
    public static void SetAccessTokenLifetimeOrDefault(this Client client, int? accessTokenLifetime)
    {
        if (accessTokenLifetime.HasValue)
            client.AccessTokenLifetime = accessTokenLifetime == -1
                ? int.MaxValue
                : accessTokenLifetime.Value;
    }

    public static void SetIdentityTokenLifetimeOrDefault(this Client client, int? identityTokenLifetime)
    {
        if (identityTokenLifetime.HasValue)
            client.IdentityTokenLifetime = identityTokenLifetime == -1
                ? int.MaxValue
                : identityTokenLifetime.Value;
    }

    public static void SetClientClaimsPrefixOrDefault(this Client client, string? maybeClientClaimsPrefix)
    {
        if (maybeClientClaimsPrefix is { } clientClaimsPrefix)
            client.ClientClaimsPrefix = clientClaimsPrefix;
    }

    public static void SetAlwaysSendClientClaimsOrDefault(this Client client, bool? alwaysSendClientClaims)
    {
        if (alwaysSendClientClaims.HasValue)
            client.AlwaysSendClientClaims = alwaysSendClientClaims.Value;
    }

    public static void SetAlwaysIncludeUserClaimsInIdTokenOrDefault(this Client client, bool? alwaysIncludeUserClaimsInIdToken)
    {
        if (alwaysIncludeUserClaimsInIdToken.HasValue)
            client.AlwaysIncludeUserClaimsInIdToken = alwaysIncludeUserClaimsInIdToken.Value;
    }

    public static List<T> MergeLists<T>(this List<T> list1, List<T> list2, Func<T, T, bool> areSame)
    {
        var result = list1;

        foreach (var item in list2.Where(item => !result.Any(x => areSame(x, item))))
        {
            result.Add(item);
        }

        return result;
    }
}
