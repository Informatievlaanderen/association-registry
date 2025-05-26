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
using Xunit;

public class GeotagsInitialisationServiceTests
{
    [Fact]
    public async Task Given_No_Verenigingen_Zonder_Geotags()
    {
        var faktory = Faktory.New();
        var query = faktory.VerenigingenZonderGeotagsQuery.Mock([]);
        var messagebus = faktory.MessageBus.Mock();

       // var sut = new GeotagsInitialisationService(query.Object, messagebus.Object, new NullLogger<GeotagsInitialisationService>());

       // await sut.StartAsync(CancellationToken.None);

        messagebus.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Given_It_Throws_An_Exception_In_The_End_Then_No_MigrationRecord_Is_Saved()
    {
        var faktory = Faktory.New();
        var query = faktory.VerenigingenZonderGeotagsQuery.Mock([]);
        var messagebus = faktory.MessageBus.Mock();

       // var sut = new GeotagsInitialisationService(query.Object, messagebus.Object, new NullLogger<GeotagsInitialisationService>());

       // await sut.StartAsync(CancellationToken.None);

        messagebus.VerifyNoOtherCalls();
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
        var postcodesFromGrarFetcher = faktory.postcodesFromGrarFetcher.MockWithPostcodes(returns: postcodes);

        var postalNutsLauInfo = fixture.Create<PostalNutsLauInfo>();
        var nutsLauFromGrarFetcher = faktory.nutsLauFromGrarFetcher.MockWithPostalNutsLauInfo(postcodes, [postalNutsLauInfo]);

        var query = faktory.VerenigingenZonderGeotagsQuery.Mock(vCodes.Select(x => x.ToString()));

        var sut = new GeotagsInitialisationService(store, martenOutbox.Object, query.Object, postcodesFromGrarFetcher.Object, nutsLauFromGrarFetcher.Object, new NullLogger<GeotagsInitialisationService>());

       await sut.ExecuteAsync(CancellationToken.None);

        foreach (var vCode in vCodes)
        {
            var commandEnvelope = new CommandEnvelope<InitialiseerGeotagsCommand>(
                new InitialiseerGeotagsCommand(vCode), CommandMetadata.ForDigitaalVlaanderenProcess);

            martenOutbox.VerifyCommand<InitialiseerGeotagsCommand>(x => x.Command == commandEnvelope.Command &&
                                                                        x.Metadata.Initiator == CommandMetadata.ForDigitaalVlaanderenProcess.Initiator &&
                                                                        x.Metadata.ExpectedVersion == CommandMetadata.ForDigitaalVlaanderenProcess.ExpectedVersion, Times.Once());
        }

        var session = store.LightweightSession();
        var actualPostalNutsLauInfo = await session.Query<PostalNutsLauInfo>().ToListAsync();
        actualPostalNutsLauInfo.Should().BeEquivalentTo([postalNutsLauInfo]);
    }
}
