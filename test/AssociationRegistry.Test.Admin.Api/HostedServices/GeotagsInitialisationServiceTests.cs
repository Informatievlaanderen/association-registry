namespace AssociationRegistry.Test.Admin.Api.HostedServices;

using AssociationRegistry.Admin.Api.HostedServices.GeotagsInitialisation;
using AssociationRegistry.DecentraalBeheer.Geotags.InitialiseerGeotags;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.NutsLau;
using AutoFixture;
using Common.AutoFixture;
using Common.Extensions;
using Common.Framework;
using Common.StubsMocksFakes;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging;
using Wolverine;
using Xunit;

public class GeotagsInitialisationServiceTests
{
    [Fact]
    public async Task Given_No_Verenigingen_Zonder_Geotags()
    {
        var faktory = Faktory.New();
        var fixture = new Fixture().CustomizeAdminApi();
        var store = await TestDocumentStoreFactory.CreateAsync("GeotagsInitialisationServiceTests");

        var martenOutbox = faktory.MartenOutbox.Mock();

        string[] postcodes = ["1500", "15001"];

        var postalNutsLauInfo = fixture.Create<PostalNutsLauInfo>();
        var nutsLauFromGrarFetcher = faktory.nutsLauFromGrarFetcher.MockWithPostalNutsLauInfo(postcodes, [postalNutsLauInfo]);

        var query = faktory.VerenigingenZonderGeotagsQuery.Mock([]);

        var sut = new GeotagsInitialisationService(store, martenOutbox.Object, query.Object,
                                                   nutsLauFromGrarFetcher.Object, new NullLogger<GeotagsInitialisationService>());

        await sut.StartAsync(CancellationToken.None);

        await sut.ExecuteTask;

        martenOutbox.Verify(x =>
                                x.SendAsync(
                                    It.IsAny<InitialiseerGeotagsCommand>(),
                                    It.IsAny<DeliveryOptions?>()),
                            Times.Never());

        var session = store.LightweightSession();

        var migrationRecord = await session.Query<GeotagMigration>().SingleOrDefaultAsync();
        migrationRecord.Should().NotBeNull();
    }

    [Fact]
    public async Task Given_It_Throws_An_Exception_In_The_End_Then_No_MigrationRecord_Is_Saved()
    {
        var faktory = Faktory.New();
        var fixture = new Fixture().CustomizeAdminApi();
        var store = await TestDocumentStoreFactory.CreateAsync("GeotagsInitialisationServiceTests");

        var martenOutbox = faktory.MartenOutbox.Mock();
        var nutsLauFromGrarFetcher = faktory.nutsLauFromGrarFetcher.Mock();

        var query = faktory.VerenigingenZonderGeotagsQuery.Mock([]);

        var sut = new GeotagsInitialisationService(store, martenOutbox.Object, query.Object,
                                                   nutsLauFromGrarFetcher.Object, new NullLogger<GeotagsInitialisationService>());

        Exception taskCanceledException = null;

        try
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(5000);

            await sut.StartAsync(cancellationTokenSource.Token);

            await sut.ExecuteTask;
        }
        catch (TaskCanceledException ex)
        {
            taskCanceledException = ex;
        }
        catch (OperationCanceledException ex)
        {
            taskCanceledException = ex;
        }

        taskCanceledException.Should().NotBeNull();

        var session = store.LightweightSession();
        var migrationRecord = await session.Query<GeotagMigration>().SingleOrDefaultAsync();
        migrationRecord.Should().BeNull();
    }

    [Fact]
    public async Task Given_Verenigingen_Met_Geotags()
    {
        var faktory = Faktory.New();
        var fixture = new Fixture().CustomizeAdminApi();
        var vCodes = fixture.CreateMany<VCode>();
        var store = await TestDocumentStoreFactory.CreateAsync("GeotagsInitialisationServiceTests");

        var martenOutbox = faktory.MartenOutbox.Mock();

        string[] postcodes = ["1500", "15001"];

        var postalNutsLauInfo = fixture.Create<PostalNutsLauInfo>();
        var nutsLauFromGrarFetcher = faktory.nutsLauFromGrarFetcher.MockWithPostalNutsLauInfo(postcodes, [postalNutsLauInfo]);

        var query = faktory.VerenigingenZonderGeotagsQuery.Mock(vCodes.Select(x => x.ToString()));

        var sut = new GeotagsInitialisationService(store, martenOutbox.Object, query.Object,
                                                   nutsLauFromGrarFetcher.Object, new NullLogger<GeotagsInitialisationService>());

        await sut.StartAsync(CancellationToken.None);

        await sut.ExecuteTask;

        foreach (var vCode in vCodes)
        {
            var commandEnvelope = new CommandEnvelope<InitialiseerGeotagsCommand>(
                new InitialiseerGeotagsCommand(vCode), CommandMetadata.ForDigitaalVlaanderenProcess);

            martenOutbox.VerifyCommandSent<InitialiseerGeotagsCommand>(x => x.Command == commandEnvelope.Command &&
                                                                            x.Metadata.Initiator == CommandMetadata
                                                                               .ForDigitaalVlaanderenProcess.Initiator &&
                                                                            x.Metadata.ExpectedVersion ==
                                                                            CommandMetadata.ForDigitaalVlaanderenProcess.ExpectedVersion,
                                                                       Times.Once());
        }

        var session = store.LightweightSession();
        var actualPostalNutsLauInfo = await session.Query<PostalNutsLauInfo>().ToListAsync();
        actualPostalNutsLauInfo.Should().BeEquivalentTo([postalNutsLauInfo]);

        var migrationRecord = await session.Query<GeotagMigration>().SingleOrDefaultAsync();
        migrationRecord.Should().NotBeNull();
    }
}
