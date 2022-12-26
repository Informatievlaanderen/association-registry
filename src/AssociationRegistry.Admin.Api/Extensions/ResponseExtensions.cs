namespace AssociationRegistry.Admin.Api.Extensions;

using Infrastructure;
using Microsoft.AspNetCore.Http;

public static class ResponseExtensions
{
    public static void AddSequenceHeader(this HttpResponse source, long sequence)
        => source.Headers[WellknownHeaderNames.Sequence] = sequence.ToString();
}
