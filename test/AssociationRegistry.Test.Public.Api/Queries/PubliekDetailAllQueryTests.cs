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

public class PubliekDetailAllQueryFixture : IAsyncLifetime
{
    public DocumentStore Store { get; set; }

    public async Task InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.Create(nameof(PubliekDetailAllQueryTests));
    }

    public async Task DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

[IntegrationTest]
public class PubliekDetailAllQueryTests : IClassFixture<PubliekDetailAllQueryFixture>, IDisposable, IAsyncDisposable
{
    private readonly IDocumentSession _session;

    public PubliekDetailAllQueryTests(PubliekDetailAllQueryFixture setupFixture)
    {
        _session = setupFixture.Store.LightweightSession();
    }

    [Fact]
    public async Task Does_Not_Return_IsUitgeschrevenUitPubliekeDatastroom_Verenigingen()
    {
        var uitgeschrevenVereniging = await StoreVereniging(_session, vereniging =>
        {
            vereniging.IsUitgeschrevenUitPubliekeDatastroom = true;
            vereniging.Status = VerenigingStatus.Actief;
        });

        var query = new PubliekDetailAllQuery(_session);

        var actual = await ConvertToListAsync(await query.ExecuteAsync(CancellationToken.None));

        actual.Should().NotContain(x => x.VCode == uitgeschrevenVereniging.VCode);
    }

    [Fact]
    public async Task Does_Not_Return_Gestopte_Verenigingen()
    {
        var gestopteVereniging = await StoreVereniging(_session, vereniging =>
        {
            vereniging.IsUitgeschrevenUitPubliekeDatastroom = false;
            vereniging.Status = VerenigingStatus.Gestopt;
        });

        var query = new PubliekDetailAllQuery(_session);

        var actual = await ConvertToListAsync(await query.ExecuteAsync(CancellationToken.None));

        actual.Should().NotContain(x => x.VCode == gestopteVereniging.VCode);
    }

    [Fact]
    public async Task Does_Return_Actieve_En_Ingeschreven_Verenigingen()
    {
        var vereniging = await StoreVereniging(_session, vereniging =>
        {
            vereniging.IsUitgeschrevenUitPubliekeDatastroom = false;
            vereniging.Status = VerenigingStatus.Actief;
        });

        var query = new PubliekDetailAllQuery(_session);

        var actual = await ConvertToListAsync(await query.ExecuteAsync(CancellationToken.None));

        actual.Should().Contain(x => x.VCode == vereniging.VCode);
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
