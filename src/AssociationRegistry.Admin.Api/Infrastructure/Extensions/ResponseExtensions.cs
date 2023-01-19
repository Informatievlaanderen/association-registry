namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Be.Vlaanderen.Basisregisters.Api.ETag;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

public static class ResponseExtensions
{
    public static void AddSequenceHeader(this HttpResponse source, long? sequence)
    {
        if (sequence is null)
            return;

        source.Headers[WellknownHeaderNames.Sequence] = sequence.ToString();
    }

    public static void AddETagHeader(this HttpResponse source, long? version)
    {
        if (version is null)
            return;

        source.Headers[HeaderNames.ETag] = new ETag(ETagType.Weak, version.ToString()!).ToString();
    }
}
