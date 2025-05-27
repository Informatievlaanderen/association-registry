namespace AssociationRegistry.Test.Admin.Api.HostedServices;

using AssociationRegistry.Admin.Api.HostedServices.GeotagsInitialisation;
using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.DecentraalBeheer.Geotags.InitialiseerGeotags;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.NutsLau;
using AutoFixture;
using Common.AutoFixture;
using Common.Extensions;
using Common.Framework;
using Common.StubsMocksFakes;
using Common.StubsMocksFakes.Faktories;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Repositories;
using Vereniging;
using Wolverine;
using Wolverine.Marten;
using Xunit;

public class GeotagsInitialisationServiceTests
{
    private readonly Faktory _faktory;
    private readonly Fixture _fixture;
    private readonly DocumentStore _store;
    private readonly IDocumentSession _session;
    private readonly GeotagMigrationRepository _geotagMigrationRepository;

    public GeotagsInitialisationServiceTests()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _faktory = Faktory.New(_fixture);
        _store = TestDocumentStoreFactory.CreateAsync("GeotagsInitialisationServiceTests").GetAwaiter().GetResult();
        _session = _store.LightweightSession();
        _geotagMigrationRepository = new GeotagMigrationRepository(_session);
    }

    [Fact]
    public async Task Given_No_Verenigingen_Zonder_Geotags()
    {
        var martenOutbox = _faktory.MartenOutbox.Mock();
        var nutsLauFromGrarFetcher = _faktory.nutsLauFromGrarFetcher.ReturnsRandomPostalNutsLauInfos();
        var query = _faktory.VerenigingenZonderGeotagsQuery.Returns(returns: []);

        var (sut, _, _, _) =
            SetupGeotagsInitialisationService(
                nutsLauFetcher: x => x.ReturnsRandomPostalNutsLauInfos(),
                verenigingenZonderGeotagsQuery: x => x.Returns(returns: []));

        await sut.StartAsync(CancellationToken.None);
        await sut.ExecuteTask;

        martenOutbox.Verify(x =>
                                x.SendAsync(
                                    It.IsAny<InitialiseerGeotagsCommand>(),
                                    It.IsAny<DeliveryOptions?>()),
                            Times.Never());

        var session = _store.LightweightSession();

        var migrationRecord = await session.Query<GeotagMigration>().SingleOrDefaultAsync();
        migrationRecord.Should().NotBeNull();
    }

    [Fact]
    public async Task Given_It_Throws_An_Exception_In_The_End_Then_No_MigrationRecord_Is_Saved()
    {
        var (sut, _, _, _) =
            SetupGeotagsInitialisationService(
                nutsLauFetcher: x => x.Throws(),
                verenigingenZonderGeotagsQuery: x => x.ReturnsRandomGeotags());

        var taskCanceledException = await CatchTaskCancelled(async () =>
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(3000);

            await sut.StartAsync(cancellationTokenSource.Token);

            await sut.ExecuteTask;
        });

        taskCanceledException.Should().NotBeNull();

        var session = _store.LightweightSession();
        var migrationRecord = await session.Query<GeotagMigration>().SingleOrDefaultAsync();
        migrationRecord.Should().BeNull();
    }

    [Fact]
    public async Task Given_Verenigingen_Met_Geotags()
    {
        var postalNutsLauInfo = _fixture.CreateMany<PostalNutsLauInfo>().ToArray();
        var vCodes = _fixture.CreateMany<VCode>().ToArray();

        var (sut, outbox, _, _) = SetupGeotagsInitialisationService(
            nutsLauFetcher: x => x.Returns(postalNutsLauInfo),
            verenigingenZonderGeotagsQuery: x =>  x.Returns(vCodes.Select(x => x.ToString())));

        await sut.StartAsync(CancellationToken.None);

        await sut.ExecuteTask;

        foreach (var vCode in vCodes)
        {
            outbox.VerifyCommandSent(new InitialiseerGeotagsCommand(vCode),
                                           CommandMetadata.ForDigitaalVlaanderenProcess,
                                           Times.Once());
        }

        var actualPostalNutsLauInfo = await _session.Query<PostalNutsLauInfo>().ToListAsync();
        actualPostalNutsLauInfo.Should().BeEquivalentTo(postalNutsLauInfo);

        var ranToCompletion = await _geotagMigrationRepository.DidMigrationAlreadyRunToCompletion(CancellationToken.None);
        ranToCompletion.Should().BeTrue();
    }

    private (GeotagsInitialisationService sut, Mock<IMartenOutbox> martenOutbox, Mock<INutsLauFromGrarFetcher> nutsLauFromGrarFetcher, Mock<IVerenigingenWithoutGeotagsQuery> query) SetupGeotagsInitialisationService(
        Func<MartenOutboxFactory, Mock<IMartenOutbox>>?  martenOutboxFactory = null,
        Func<NutsLauFromGrarFetcherFactory, Mock<INutsLauFromGrarFetcher>>? nutsLauFetcher = null,
        Func<VerenigingenZonderGeotagsQueryFactory, Mock<IVerenigingenWithoutGeotagsQuery>>? verenigingenZonderGeotagsQuery = null
    )
    {
        martenOutboxFactory ??= factory => factory.Mock();
        nutsLauFetcher ??= factory => factory.Mock();
        verenigingenZonderGeotagsQuery ??= factory => factory.Returns([]);

        var martenOutbox = martenOutboxFactory(_faktory.MartenOutbox);
        var nutsLauFromGrarFetcher = nutsLauFetcher(_faktory.nutsLauFromGrarFetcher);
        var query = verenigingenZonderGeotagsQuery(_faktory.VerenigingenZonderGeotagsQuery);

        var sut = new GeotagsInitialisationService(_store, martenOutbox.Object, query.Object,
                                               nutsLauFromGrarFetcher.Object, new NullLogger<GeotagsInitialisationService>());

        return (sut, martenOutbox, nutsLauFromGrarFetcher, query);
    }

    private static async Task<Exception?> CatchTaskCancelled(Func<Task> action)
    {
        Exception taskCanceledException = null;

        try
        {
            await action();
        }
        catch (TaskCanceledException ex)
        {
            taskCanceledException = ex;
        }
        catch (OperationCanceledException ex)
        {
            taskCanceledException = ex;
        }

        return taskCanceledException;
    }
}
