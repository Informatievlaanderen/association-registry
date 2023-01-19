namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Infrastructure;
using Microsoft.AspNetCore.Http;

public static class ResponseExtensions
{
    public static void AddSequenceHeader(this HttpResponse source, long? sequence)
    {
        if (sequence is null)
            return;

        source.Headers[WellknownHeaderNames.Sequence] = sequence.ToString();
    }
}
