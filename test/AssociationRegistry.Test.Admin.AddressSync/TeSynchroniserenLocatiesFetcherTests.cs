namespace AssociationRegistry.Test.Admin.AddressSync;

using AssociationRegistry.Admin.AddressSync;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Grar;
using Grar.AddressSync;
using Grar.Models;
using Marten;
using Moq;
using Vereniging;
using Vereniging.Bronnen;

public class TeSynchroniserenLocatiesFetcherTests
{
    [Fact]
    public async Task Given_No_LocatieLookupDocumenten_Returns_Empty()
    {
        var store = await TestDocumentStoreFactory.Create("addresssync");

        var session = store.LightweightSession();

        var teSynchroniserenLocatiesFetcher = new TeSynchroniserenLocatiesFetcher(Mock.Of<IGrarClient>());

        var locaties = await teSynchroniserenLocatiesFetcher.GetTeSynchroniserenLocaties(session, CancellationToken.None);

        locaties.Should().BeEmpty();
    }

    [Fact]
    public async Task Given_1_LocatieLookupDocumenten_Returns_1_Locatie()
    {
        var fixture = new Fixture().CustomizeDomain();
        var store = await TestDocumentStoreFactory.Create("addresssync");

        var session = store.LightweightSession();

        var document = new LocatieLookupDocument
        {
            VCode = "VCode1",
            AdresId = "123",
            LocatieId = 1,
            Id = Guid.NewGuid().ToString(),
        };

        var grarClient = new Mock<IGrarClient>();

        var addressDetailResponse = fixture.Create<AddressDetailResponse>() with
        {
            AdresId = new Registratiedata.AdresId(Adresbron.AR, document.AdresId),
        };

        grarClient
           .Setup(x => x.GetAddressById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(addressDetailResponse);

        session.Store(document);
        await session.SaveChangesAsync();

        var teSynchroniserenLocatiesFetcher = new TeSynchroniserenLocatiesFetcher(grarClient.Object);

        var locaties = await teSynchroniserenLocatiesFetcher.GetTeSynchroniserenLocaties(session, CancellationToken.None);

        locaties.Should().BeEquivalentTo([
            new TeSynchroniserenLocatieAdresMessage(document.VCode, new List<LocatieWithAdres>()
            {
                new LocatieWithAdres(document.LocatieId, addressDetailResponse),
            }, ""),
        ], options => options.Excluding(x => x.IdempotenceKey));
    }

    [Fact]
    public async Task Given_Several_LocatieLookupDocumenten_Returns_Grouped_Messages()
    {
        var fixture = new Fixture().CustomizeDomain();
        var store = await TestDocumentStoreFactory.Create("addresssync");
        var grarClient = new Mock<IGrarClient>();

        var session = store.LightweightSession();

        var vCode1 = "VCode1";
        var vCode2 = "VCode2";

        var adresId1 = "123";
        var adresId2 = "456";
        var adresId3 = "789";

        var address1DetailResponse = SetUpAddressDetailResponse(fixture, grarClient, adresId1);
        var address2DetailResponse = SetUpAddressDetailResponse(fixture, grarClient, adresId2);
        var address3DetailResponse = SetUpAddressDetailResponse(fixture, grarClient, adresId3);

        StoreLocatieLookupDocument(session, vCode1, adresId1, 1);
        StoreLocatieLookupDocument(session, vCode2, adresId1, 1);
        StoreLocatieLookupDocument(session, vCode1, adresId2, 2);
        StoreLocatieLookupDocument(session, vCode2, adresId3, 2);

        await session.SaveChangesAsync();

        var teSynchroniserenLocatiesFetcher = new TeSynchroniserenLocatiesFetcher(grarClient.Object);

        var locaties = await teSynchroniserenLocatiesFetcher.GetTeSynchroniserenLocaties(session, CancellationToken.None);

        // Verify only once per unique adres id we go fetch adres from adresssenregister
        grarClient
           .Verify(x => x.GetAddressById(adresId1, It.IsAny<CancellationToken>()), Times.Once());

        locaties.Should().BeEquivalentTo([
            new TeSynchroniserenLocatieAdresMessage(vCode1, new List<LocatieWithAdres>()
            {
                new LocatieWithAdres(1, address1DetailResponse),
                new LocatieWithAdres(2, address2DetailResponse),
            }, ""),
            new TeSynchroniserenLocatieAdresMessage(vCode2, new List<LocatieWithAdres>()
            {
                new LocatieWithAdres(1, address1DetailResponse),
                new LocatieWithAdres(2, address3DetailResponse),
            }, ""),
        ], options => options.Excluding(x => x.IdempotenceKey));
    }

    private static AddressDetailResponse SetUpAddressDetailResponse(Fixture fixture, Mock<IGrarClient> grarClient, string adresId1)
    {
        var response = fixture.Create<AddressDetailResponse>() with
        {
            AdresId = new Registratiedata.AdresId(Adresbron.AR, adresId1),
        };

        grarClient
           .Setup(x => x.GetAddressById(adresId1, It.IsAny<CancellationToken>()))
           .ReturnsAsync(response);

        return response;
    }

    private LocatieLookupDocument StoreLocatieLookupDocument(IDocumentSession session, string VCode, string AdresId, int LocatieId)
    {
        var locatieLookupDocument = new LocatieLookupDocument
        {
            VCode = VCode,
            AdresId = AdresId,
            LocatieId = LocatieId,
            Id = Guid.NewGuid().ToString(),
        };

        session.Store(locatieLookupDocument);

        return locatieLookupDocument;
    }
}
