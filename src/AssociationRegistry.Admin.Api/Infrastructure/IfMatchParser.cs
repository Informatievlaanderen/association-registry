namespace AssociationRegistry.Admin.Api.Infrastructure;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

public static class IfMatchParser
{
    public static long? ParseIfMatch(string? ifMatch)
    {
        if (ifMatch is null) return null;
        var tryParse = EntityTagHeaderValue.TryParse(ifMatch, out var parsedEtag);
        if (!tryParse)
            throw new BadHttpRequestException("Etag header bevat een ongeldige waarde");

        var tryParseNumber = long.TryParse(parsedEtag.Tag.Value.Trim('"'), out var etagAsNumber);
        if (!tryParseNumber || etagAsNumber < 0 || !parsedEtag.IsWeak)
            throw new BadHttpRequestException("Etag header bevat een ongeldige waarde");

        return etagAsNumber;
    }
}
