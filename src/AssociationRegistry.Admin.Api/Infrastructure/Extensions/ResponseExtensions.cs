namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using AssociationRegistry.Admin.Api.Infrastructure;
using Microsoft.AspNetCore.Http;

public static class ResponseExtensions
{
    public static void AddSequenceHeader(this HttpResponse source, long sequence)
        => source.Headers[WellknownHeaderNames.Sequence] = sequence.ToString();
}
