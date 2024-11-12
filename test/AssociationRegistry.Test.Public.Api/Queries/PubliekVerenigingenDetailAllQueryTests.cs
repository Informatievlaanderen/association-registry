namespace AssociationRegistry.Test.Public.Api.Queries;

using AssociationRegistry.Public.Api.Queries;
using AssociationRegistry.Public.Schema.Constants;
using AssociationRegistry.Public.Schema.Detail;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Marten;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

public class PubliekVerenigingenDetailAllQueryFixture : IAsyncLifetime
{
    public DocumentStore Store { get; set; }

    public async Task InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.Create(nameof(PubliekVerenigingenDetailAllQueryTests));
    }

    public async Task DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

[IntegrationTest]
public class PubliekVerenigingenDetailAllQueryTests : IClassFixture<PubliekVerenigingenDetailAllQueryFixture>, IDisposable, IAsyncDisposable
{
    private readonly IDocumentSession _session;

    public PubliekVerenigingenDetailAllQueryTests(PubliekVerenigingenDetailAllQueryFixture setupFixture)
    {
        _session = setupFixture.Store.LightweightSession();
    }

    [Fact]
    public async Task Does_Return_Verwijderde_Verenigingen()
    {
        var gestopteVereniging = await StoreVereniging(_session, vereniging =>
        {
            vereniging.Deleted = true;
        });

        var query = new PubliekVerenigingenDetailAllQuery(_session);

        var actual = await ConvertToListAsync(await query.ExecuteAsync(CancellationToken.None));

        actual.Should().Contain(x => x.VCode == gestopteVereniging.VCode);
    }

    private async Task<PubliekVerenigingDetailDocument> StoreVereniging(IDocumentSession session, Action<PubliekVerenigingDetailDocument> func)
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = fixture.Create<PubliekVerenigingDetailDocument>();
        func(vereniging);

        session.Store(vereniging);
        await session.SaveChangesAsync();

        return vereniging;
    }

    private async Task<List<T>> ConvertToListAsync<T>(IAsyncEnumerable<T> asyncEnumerable)
    {
        var list = new List<T>();

        await foreach (var item in asyncEnumerable)
        {
            list.Add(item);
        }

        return list;
    }

    public void Dispose()
    {
        _session.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _session.DisposeAsync();
    }
}
