namespace AssociationRegistry.Test.Admin.AddressSync.When_Fetching_Te_Synchroniseren_Locaties_Zonder_Adres_Match;

using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.Schema.Locaties;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

public class Given_No_LocatieZonderAdresMatchDocumenten
{
    private LocatieZonderAdresMatchDocument[] _locaties;

    public Given_No_LocatieZonderAdresMatchDocumenten()
    {
        var store = TestDocumentStoreFactory.CreateAsync(nameof(Given_No_LocatieZonderAdresMatchDocumenten)).GetAwaiter()
                                            .GetResult();

        using var session = store.LightweightSession();

        var sut =
            new TeSynchroniserenLocatiesZonderAdresMatchFetcher(
                NullLogger<TeSynchroniserenLocatiesZonderAdresMatchFetcher>.Instance);

        _locaties = sut.GetTeSynchroniserenLocatiesZonderAdresMatch(session, CancellationToken.None).GetAwaiter()
                       .GetResult();
    }

    [Fact]
    public void Given_No_LocatieLookupDocumenten_Returns_Empty()
    {
        _locaties.Should().BeEmpty();
    }
}
