namespace AssociationRegistry.Test.Admin.AddressSync.When_Fetching_Te_Synchroniseren_Locaties_Zonder_Adres_Match;

using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.Schema.Locaties;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

public class Given_1_LocatieZonderAdresMatchDocumenten
{
    private LocatieZonderAdresMatchDocument[] _locaties;
    private LocatieZonderAdresMatchDocument? _document;

    public Given_1_LocatieZonderAdresMatchDocumenten()
    {
        var fixture = new Fixture().CustomizeDomain();

        using var store = TestDocumentStoreFactory.CreateAsync(nameof(Given_1_LocatieZonderAdresMatchDocumenten))
                                                  .GetAwaiter().GetResult();

        using var session = store.LightweightSession();

        _document = fixture.Create<LocatieZonderAdresMatchDocument>();

        session.Store(_document);
        session.SaveChangesAsync().GetAwaiter().GetResult();

        var sut =
            new TeSynchroniserenLocatiesZonderAdresMatchFetcher(
                NullLogger<TeSynchroniserenLocatiesZonderAdresMatchFetcher>.Instance);

        _locaties = sut.GetTeSynchroniserenLocatiesZonderAdresMatch(session, CancellationToken.None).GetAwaiter()
                       .GetResult();
    }

    [Fact]
    public void Given_1_LocatieZonderAdresMatchDocumenten_Returns_1_Locatie()
    {
        _locaties.Should().BeEquivalentTo([_document]);
    }
}
