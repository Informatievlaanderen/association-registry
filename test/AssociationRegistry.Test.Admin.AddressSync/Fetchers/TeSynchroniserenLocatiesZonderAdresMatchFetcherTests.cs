namespace AssociationRegistry.Test.Admin.AddressSync.Fetchers;

using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.Schema.Locaties;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Events;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Integrations.Grar.Clients;
using AssociationRegistry.Integrations.Grar.Exceptions;
using AssociationRegistry.Integrations.Grar.Integration.Messages;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class TeSynchroniserenLocatiesZonderAdresMatchFetcherTests
{
    [Fact]
    public async ValueTask Given_No_LocatieLookupDocumenten_Returns_Empty()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(nameof(TeSynchroniserenLocatiesZonderAdresMatchFetcherTests));
        await using var session = store.LightweightSession();

        var fetcher =
            new TeSynchroniserenLocatiesZonderAdresMatchFetcher(NullLogger<TeSynchroniserenLocatiesZonderAdresMatchFetcher>.Instance);

        var locaties = await fetcher.GetTeSynchroniserenLocatiesZonderAdresMatch(session, CancellationToken.None);

        locaties.Should().BeEmpty();
    }

    [Fact]
    public async ValueTask Given_1_LocatieZonderAdresMatchDocumenten_Returns_1_Locatie()
    {
        var fixture = new Fixture().CustomizeDomain();

        await using var store = await TestDocumentStoreFactory.CreateAsync(nameof(TeSynchroniserenLocatiesZonderAdresMatchFetcherTests));
        await using var session = store.LightweightSession();

        var document = fixture.Create<LocatieZonderAdresMatchDocument>();

        session.Store(document);
        await session.SaveChangesAsync();

        var fetcher =
            new TeSynchroniserenLocatiesZonderAdresMatchFetcher(NullLogger<TeSynchroniserenLocatiesZonderAdresMatchFetcher>.Instance);

        var locaties = await fetcher.GetTeSynchroniserenLocatiesZonderAdresMatch(session, CancellationToken.None);

        locaties.Should().BeEquivalentTo([document]);
    }

    [Fact]
    public async ValueTask Given_Several_LocatieZonderAdresMatchDocumenten_Returns_Grouped_Messages()
    {
        var fixture = new Fixture().CustomizeDomain();

        await using var store = await TestDocumentStoreFactory.CreateAsync(nameof(TeSynchroniserenLocatiesZonderAdresMatchFetcherTests));
        await using var session = store.LightweightSession();

        var document1 = fixture.Create<LocatieZonderAdresMatchDocument>();
        var document2 = fixture.Create<LocatieZonderAdresMatchDocument>();
        var document3 = fixture.Create<LocatieZonderAdresMatchDocument>();

        session.Store(document1);
        session.Store(document2);
        session.Store(document3);
        await session.SaveChangesAsync();

        var fetcher =
            new TeSynchroniserenLocatiesZonderAdresMatchFetcher(NullLogger<TeSynchroniserenLocatiesZonderAdresMatchFetcher>.Instance);

        var locaties = await fetcher.GetTeSynchroniserenLocatiesZonderAdresMatch(session, CancellationToken.None);


        locaties.Should().BeEquivalentTo([document1,document2,document3]);
    }
}
