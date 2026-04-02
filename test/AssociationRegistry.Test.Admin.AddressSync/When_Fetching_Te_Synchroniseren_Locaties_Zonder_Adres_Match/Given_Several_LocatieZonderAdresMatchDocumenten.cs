namespace AssociationRegistry.Test.Admin.AddressSync.When_Fetching_Te_Synchroniseren_Locaties_Zonder_Adres_Match;

using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.Schema.Locaties;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

public class Given_Several_LocatieZonderAdresMatchDocumenten
{
    private LocatieZonderAdresMatchDocument[] _locaties;
    private LocatieZonderAdresMatchDocument? _document1;
    private LocatieZonderAdresMatchDocument? _document2;
    private LocatieZonderAdresMatchDocument? _document3;

    public Given_Several_LocatieZonderAdresMatchDocumenten()
    {
        var fixture = new Fixture().CustomizeDomain();

        using var store = TestDocumentStoreFactory.CreateAsync(nameof(Given_Several_LocatieZonderAdresMatchDocumenten))
                                                  .GetAwaiter().GetResult();

        using var session = store.LightweightSession();

        _document1 = fixture.Create<LocatieZonderAdresMatchDocument>();
        _document2 = fixture.Create<LocatieZonderAdresMatchDocument>();
        _document3 = fixture.Create<LocatieZonderAdresMatchDocument>();

        session.Store(_document1);
        session.Store(_document2);
        session.Store(_document3);
        session.SaveChangesAsync().GetAwaiter().GetResult();

        var fetcher =
            new TeSynchroniserenLocatiesZonderAdresMatchFetcher(
                NullLogger<TeSynchroniserenLocatiesZonderAdresMatchFetcher>.Instance);

        _locaties = fetcher.GetTeSynchroniserenLocatiesZonderAdresMatch(session, CancellationToken.None).GetAwaiter()
                           .GetResult();
    }

    [Fact]
    public void Given_Several_LocatieZonderAdresMatchDocumenten_Returns_Grouped_Messages()
    {
        _locaties.Should().BeEquivalentTo([_document1, _document2, _document3]);
    }
}
