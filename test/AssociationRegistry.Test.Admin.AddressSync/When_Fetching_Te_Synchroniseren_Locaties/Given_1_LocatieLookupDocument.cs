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
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class Given_1_LocatieLookupDocument
{
    private IEnumerable<TeSynchroniserenLocatieAdresMessage> _locaties;
    private LocatieLookupDocument _document;
    private AddressDetailResponse _addressDetailResponse;

    public Given_1_LocatieLookupDocument()
    {
        var fixture = new Fixture().CustomizeDomain();

        using var store = TestDocumentStoreFactory.CreateAsync(nameof(Given_1_LocatieLookupDocument))
                                                  .GetAwaiter().GetResult();

        using var session = store.LightweightSession();

        _document = new LocatieLookupDocument
        {
            VCode = "VCode1",
            AdresId = "123",
            LocatieId = 1,
            Id = Guid.NewGuid().ToString(),
        };

        var grarClient = new Mock<IGrarClient>();

        _addressDetailResponse = fixture.Create<AddressDetailResponse>() with
        {
            AdresId = new Registratiedata.AdresId(Adresbron.AR, _document.AdresId),
        };

        grarClient
           .Setup(x => x.GetAddressById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(_addressDetailResponse);

        session.Store(_document);
        session.SaveChangesAsync().GetAwaiter().GetResult();

        var sut =
            new TeSynchroniserenLocatiesFetcher(grarClient.Object,
                                                NullLogger<TeSynchroniserenLocatiesFetcher>.Instance);

        _locaties = sut.GetTeSynchroniserenLocaties(session, CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_Locaties_Should_Contain_1_Locatie()
    {
        _locaties.Should().BeEquivalentTo([
                                              new TeSynchroniserenLocatieAdresMessage(_document.VCode,
                                                  new List<LocatieWithAdres>
                                                  {
                                                      new(_document.LocatieId, _addressDetailResponse),
                                                  },
                                                  IdempotenceKey: ""),
                                          ],
                                          config: options => options.Excluding(x => x.IdempotenceKey));
    }
}
