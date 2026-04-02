namespace AssociationRegistry.Test.Admin.AddressSync.When_Fetching_Te_Synchroniseren_Locaties;

using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.Schema.Locaties;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging.Adressen;
using Events;
using FluentAssertions;
using Integrations.Grar.Clients;
using Integrations.Grar.Integration.Messages;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class Given_Several_LocatieLookupDocumenten
{
    private IEnumerable<TeSynchroniserenLocatieAdresMessage> _locaties;
    private string _vCode1;
    private string _vCode2;
    private AddressDetailResponse _address1DetailResponse;
    private AddressDetailResponse _address2DetailResponse;
    private AddressDetailResponse _address3DetailResponse;

    public Given_Several_LocatieLookupDocumenten()
    {
        var fixture = new Fixture().CustomizeDomain();

        using var store = TestDocumentStoreFactory.CreateAsync(nameof(Given_Several_LocatieLookupDocumenten))
                                                  .GetAwaiter().GetResult();

        var grarClient = new Mock<IGrarClient>();

        using var session = store.LightweightSession();

        _vCode1 = "VCode1";
        _vCode2 = "VCode2";

        var adresId1 = "123";
        var adresId2 = "456";
        var adresId3 = "789";

        _address1DetailResponse = SetUpAddressDetailResponse(fixture, grarClient, adresId1);
        _address2DetailResponse = SetUpAddressDetailResponse(fixture, grarClient, adresId2);
        _address3DetailResponse = SetUpAddressDetailResponse(fixture, grarClient, adresId3);

        StoreLocatieLookupDocument(session, _vCode1, adresId1, LocatieId: 1);
        StoreLocatieLookupDocument(session, _vCode2, adresId1, LocatieId: 1);
        StoreLocatieLookupDocument(session, _vCode1, adresId2, LocatieId: 2);
        StoreLocatieLookupDocument(session, _vCode2, adresId3, LocatieId: 2);

        session.SaveChangesAsync().GetAwaiter().GetResult();

        var teSynchroniserenLocatiesFetcher =
            new TeSynchroniserenLocatiesFetcher(grarClient.Object,
                                                NullLogger<TeSynchroniserenLocatiesFetcher>.Instance);

        _locaties = teSynchroniserenLocatiesFetcher.GetTeSynchroniserenLocaties(session, CancellationToken.None)
                                                   .GetAwaiter().GetResult();

        // Verify only once per unique adres id we go fetch adres from adresssenregister
        grarClient
           .Verify(expression: x => x.GetAddressById(adresId1, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public void Then_Returns_Grouped_Messages()
    {
        _locaties.Should().BeEquivalentTo([
                                              new TeSynchroniserenLocatieAdresMessage(_vCode1,
                                                  new List<LocatieWithAdres>
                                                  {
                                                      new(LocatieId: 1, _address1DetailResponse),
                                                      new(LocatieId: 2, _address2DetailResponse),
                                                  },
                                                  IdempotenceKey: ""),
                                              new TeSynchroniserenLocatieAdresMessage(_vCode2,
                                                  new List<LocatieWithAdres>
                                                  {
                                                      new(LocatieId: 1, _address1DetailResponse),
                                                      new(LocatieId: 2, _address3DetailResponse),
                                                  },
                                                  IdempotenceKey: ""),
                                          ],
                                          config: options => options.Excluding(x => x.IdempotenceKey));
    }

    private static AddressDetailResponse SetUpAddressDetailResponse(
        Fixture fixture,
        Mock<IGrarClient> grarClient,
        string adresId1)
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

    private LocatieLookupDocument StoreLocatieLookupDocument(
        IDocumentSession session,
        string VCode,
        string AdresId,
        int LocatieId)
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
