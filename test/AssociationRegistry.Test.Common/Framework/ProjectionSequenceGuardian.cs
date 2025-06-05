namespace AssociationRegistry.Test.Common.Framework;

using FluentAssertions;
using Marten;
using Nest;

public static class ProjectionSequenceGuardian
{


    public static async Task EnsureAllProjectionsAreUpToDate(IDocumentStore projectionsDocumentStore, long maxSequence, IElasticClient? elasticClient)
    {
        var (sequencesPerProjection, reachedSequence) = await HaveAllProjectionsReachedHighwaterMark(projectionsDocumentStore, maxSequence);
        var counter = 0;

        while (!reachedSequence && counter < 20)
        {
            counter++;

            await Task.Delay(500 + (100 * counter));

            (sequencesPerProjection, reachedSequence) = await HaveAllProjectionsReachedHighwaterMark(projectionsDocumentStore, maxSequence);
        }

        sequencesPerProjection.Should()
                              .AllSatisfy(x => x.Value.Should()
                                                .BeGreaterThanOrEqualTo(
                                                     maxSequence, $"Because we want projection {x.Key} to be up to date"));

        await elasticClient.Indices.RefreshAsync(Indices.AllIndices);

        if(elasticClient is not null)
            await elasticClient.Indices.ForceMergeAsync(Indices.AllIndices, fm => fm
                                                           .MaxNumSegments(1));

    }

    private static async Task<(Dictionary<string, long> sequencesPerProjection, bool reachedSequence)> HaveAllProjectionsReachedHighwaterMark(IDocumentStore projectionsDocumentStore, long maxSequence)
    {
        var sequencesPerProjection = await SequencesPerProjection(projectionsDocumentStore);

        var reachedSequence = Enumerable.All<KeyValuePair<string, long>>(sequencesPerProjection, x => x.Value >= maxSequence);


        return (sequencesPerProjection, reachedSequence);
    }

    private static async Task<Dictionary<string, long>> SequencesPerProjection(IDocumentStore projectionsDocumentStore)
    {
        Dictionary<string, long> sequencesPerProjection;
        var allProjectionProgress = await projectionsDocumentStore.Advanced.AllProjectionProgress();

        sequencesPerProjection = allProjectionProgress
                                .ToList()
                                .ToDictionary(x => x.ShardName, x => x.Sequence);

        return sequencesPerProjection;
    }
}
