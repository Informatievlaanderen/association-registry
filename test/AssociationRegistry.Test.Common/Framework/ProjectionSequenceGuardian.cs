namespace AssociationRegistry.Test.Common.Framework;

using Elastic.Clients.Elasticsearch;
using FluentAssertions;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.Extensions.Logging;

public static class ProjectionSequenceGuardian
{
    public static async Task EnsureAllProjectionsAreUpToDate(
        IDocumentStore projectionsDocumentStore,
        string serviceName,
        long maxSequence,
        ILogger logger,
        ElasticsearchClient? elasticClient = null)
    {
        var (sequencesPerProjection, reachedSequence) = await HaveAllProjectionsReachedHighwaterMark(
            projectionsDocumentStore,
            maxSequence,
            serviceName
        );
        var counter = 0;

        while (!reachedSequence && counter < 20)
        {
            counter++;

            await Task.Delay(500 + (100 * counter));

            (sequencesPerProjection, reachedSequence) = await HaveAllProjectionsReachedHighwaterMark(
                projectionsDocumentStore,
                maxSequence,
                serviceName
            );
        }

        sequencesPerProjection
            .Should()
            .AllSatisfy(x =>
                x.Value.Should()
                    .BeGreaterThanOrEqualTo(maxSequence, $"Because we want projection {x.Key} to be up to date")
            );

        if (elasticClient != null)
        {
            await elasticClient.Indices.RefreshAsync(Indices.All);
            await elasticClient.Indices.ForcemergeAsync(Indices.All, fm => fm.MaxNumSegments(1));
        }
    }

    private static async Task<(Dictionary<string, long> sequencesPerProjection, bool reachedSequence)>
        HaveAllProjectionsReachedHighwaterMark(
            IDocumentStore projectionsDocumentStore,
            long maxSequence,
            string serviceName)
    {
        var sequencesPerProjection = (await SequencesPerProjection(projectionsDocumentStore))
            .Where(x => x.Key.StartsWith(serviceName))
            .ToDictionary();

        var reachedSequence = Enumerable.All<KeyValuePair<string, long>>(
            sequencesPerProjection,
            x => x.Value >= maxSequence
        );

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

        allProjectionProgress.Should().NotBeNullOrEmpty().And.Subject.Count().Should().BeGreaterThan(1);

        sequencesPerProjection = allProjectionProgress.ToList().ToDictionary(x => x.ShardName, x => x.Sequence);

        return sequencesPerProjection;
    }
}
