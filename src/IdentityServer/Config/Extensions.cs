namespace IdentityServer.Config;

using Duende.IdentityServer.Models;

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
