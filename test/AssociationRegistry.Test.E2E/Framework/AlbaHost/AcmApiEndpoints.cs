namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Acm.Api.VerenigingenPerInsz;
using Alba;
using Marten;
using Microsoft.Extensions.DependencyInjection;

public static class AcmApiEndpoints
{
    public static async Task<VerenigingenPerInszResponse> GetVerenigingenPerInsz(this IAlbaHost source, VerenigingenPerInszRequest request, long? expectedSequence)
    {
        await WaitForExpectedSequence(source, expectedSequence, "AssociationRegistry.Acm.Api.Projections.VerenigingenPerInszProjection:All");

        return await source.PostJson(request, $"/v1/verenigingen", JsonStyle.Mvc)
                     .Receive<VerenigingenPerInszResponse>();
    }

    private static async Task WaitForExpectedSequence(IAlbaHost source, long? expectedSequence, string publiekverenigingzoekendocumentAll)
    {
        var store = source.Services.GetRequiredService<IDocumentStore>();
        var result = (await store.Advanced
                                 .AllProjectionProgress()).SingleOrDefault(x => x.ShardName == publiekverenigingzoekendocumentAll)?.Sequence;


        bool reachedSequence = result >= expectedSequence;
        var counter = 0;
        while (!reachedSequence && counter < 10)
        {
            counter++;
            await Task.Delay(500);
            result = (await store.Advanced
                                 .AllProjectionProgress()).SingleOrDefault(x => x.ShardName == publiekverenigingzoekendocumentAll)?.Sequence;

            reachedSequence = result >= expectedSequence;
        }
    }
}
