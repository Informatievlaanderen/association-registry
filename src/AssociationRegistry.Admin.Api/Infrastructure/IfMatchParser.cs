namespace AssociationRegistry.Admin.Api.Infrastructure;

using Microsoft.Net.Http.Headers;

public static class IfMatchParser
{
    public static long? ParseIfMatch(string? ifMatch)
    {
        if (ifMatch is null) return null;
        var tryParse = EntityTagHeaderValue.TryParse(ifMatch, out var parsedEtag);

        if (!tryParse)
            throw new EtagHeaderIsInvalidException();

        var tryParseNumber = long.TryParse(parsedEtag.Tag.Value.Trim(trimChar: '"'), out var etagAsNumber);

        if (!tryParseNumber || etagAsNumber < 0 || !parsedEtag.IsWeak)
            throw new EtagHeaderIsInvalidException();

        return etagAsNumber;
    }

    public class EtagHeaderIsInvalidException : BadHttpRequestException
    {
        public EtagHeaderIsInvalidException() : base(
            message: "Etag header bevat een ongeldige waarde, gelieve formaat 'W/\"[0-9]*\"' te gebruiken",
            StatusCodes.Status412PreconditionFailed)
        {
        }
    }
}
