namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Microsoft.Extensions.Primitives;
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

        var responseHeaders = source.GetTypedHeaders();
        responseHeaders.ETag = new EntityTagHeaderValue(new StringSegment($"\"{version}\""), isWeak: true);
    }

    public static void AddLocationHeader(this HttpResponse source, string vCode, string location, string id, string baseUrl)
    {
        if (string.IsNullOrEmpty(id))
            return;

        var responseHeaders = source.GetTypedHeaders();
        responseHeaders.Location = new Uri($"{baseUrl}/v1/verenigingen/{vCode}/{location}/{id}");
    }
}
