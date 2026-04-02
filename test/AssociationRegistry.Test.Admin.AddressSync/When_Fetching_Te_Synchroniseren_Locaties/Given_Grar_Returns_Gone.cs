namespace AssociationRegistry.Test.Admin.AddressSync.When_Fetching_Te_Synchroniseren_Locaties;

using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.Schema.Locaties;
using AssociationRegistry.Grar.Models;
using FluentAssertions;
using Integrations.Grar.Clients;
using Integrations.Grar.Exceptions;
using Integrations.Grar.Integration.Messages;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class Given_Grar_Returns_Gone
{
    private IEnumerable<TeSynchroniserenLocatieAdresMessage> _locaties;
    private LocatieLookupDocument _document;

    public Given_Grar_Returns_Gone()
    {
        var store = TestDocumentStoreFactory.CreateAsync(nameof(Given_Grar_Returns_Gone)).GetAwaiter()
                                            .GetResult();

        using var session = store.LightweightSession();

        _document = new LocatieLookupDocument
        {
            VCode = "VCode1",
            AdresId = "123",
            LocatieId = 1,
            Id = Guid.NewGuid().ToString(),
        };

        var grarClient = new Mock<IGrarClient>();

        grarClient
           .Setup(x => x.GetAddressById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ThrowsAsync(new AdressenregisterReturnedGoneStatusCode());

        session.Store(_document);
        session.SaveChangesAsync().GetAwaiter().GetResult();

        var sut =
            new TeSynchroniserenLocatiesFetcher(grarClient.Object,
                                                NullLogger<TeSynchroniserenLocatiesFetcher>.Instance);

        _locaties = sut.GetTeSynchroniserenLocaties(session, CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_Returns_Gone_Then_Message_Has_Locatie_With_Null_Adres()
    {
        _locaties.Should().BeEquivalentTo([
                                              new TeSynchroniserenLocatieAdresMessage(_document.VCode,
                                                  new List<LocatieWithAdres>
                                                  {
                                                      new(_document.LocatieId, Adres: null),
                                                  },
                                                  IdempotenceKey: ""),
                                          ],
                                          config: options => options.Excluding(x => x.IdempotenceKey));
    }
}
