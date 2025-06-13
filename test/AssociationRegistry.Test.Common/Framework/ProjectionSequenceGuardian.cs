namespace AssociationRegistry.Test.Common.Framework;

using Admin.Schema.Search;
using FluentAssertions;
using Marten;
using Marten.Events.Daemon;
using Microsoft.Extensions.Logging;
using Nest;

public static class ProjectionSequenceGuardian
{
    public static async Task EnsureAllProjectionsAreUpToDate(IDocumentStore projectionsDocumentStore, long maxSequence, IElasticClient elasticClient, ILogger logger)
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
        var counter = 0;
        var hasAtLeastOneProjection = false;

        Dictionary<string, long> sequencesPerProjection;
        IReadOnlyList<ShardState> allProjectionProgress = null;

        while (!hasAtLeastOneProjection && counter < 20)
        {
            counter++;

            await Task.Delay(500 + (100 * counter));

            allProjectionProgress = await projectionsDocumentStore.Advanced.AllProjectionProgress();
            hasAtLeastOneProjection = allProjectionProgress.Count > 1; // because high watermark is always there, so we need at least one more projection
        }

        allProjectionProgress.Should().NotBeNullOrEmpty()
                             .And.Subject.Count().Should().BeGreaterThan(1);

        sequencesPerProjection = allProjectionProgress
                                .ToList()
                                .ToDictionary(x => x.ShardName, x => x.Sequence);

        return sequencesPerProjection;
    }
}
