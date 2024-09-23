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
public class PubliekDetailAllQueryTests : IClassFixture<PubliekDetailAllQueryFixture>
{
    private readonly PubliekDetailAllQueryFixture _setupFixture;
    private readonly Fixture _autoFixture;

    public PubliekDetailAllQueryTests(PubliekDetailAllQueryFixture setupFixture)
    {
        _autoFixture = new Fixture().CustomizeDomain();
        _setupFixture = setupFixture;
    }

    [Fact]
    public async Task Does_Not_Return_IsUitgeschrevenUitPubliekeDatastroom_Verenigingen()
    {
        await using var session = _setupFixture.Store.LightweightSession();

        var uitgeschrevenVereniging = _autoFixture.Create<PubliekVerenigingDetailDocument>();
        uitgeschrevenVereniging.IsUitgeschrevenUitPubliekeDatastroom = true;
        uitgeschrevenVereniging.Status = VerenigingStatus.Actief;

        session.Store(uitgeschrevenVereniging);
        await session.SaveChangesAsync();

        var query = new PubliekDetailAllQuery(session);

        var actual = await ConvertToListAsync(await query.ExecuteAsync(CancellationToken.None));

        actual.Should().NotContain(x => x.VCode == uitgeschrevenVereniging.VCode);
    }

    [Fact]
    public async Task Does_Not_Return_Gestopte_Verenigingen()
    {
        await using var session = _setupFixture.Store.LightweightSession();

        var uitgeschrevenVereniging = _autoFixture.Create<PubliekVerenigingDetailDocument>();
        uitgeschrevenVereniging.IsUitgeschrevenUitPubliekeDatastroom = false;
        uitgeschrevenVereniging.Status = VerenigingStatus.Gestopt;

        session.Store(uitgeschrevenVereniging);
        await session.SaveChangesAsync();

        var query = new PubliekDetailAllQuery(session);

        var actual = await ConvertToListAsync(await query.ExecuteAsync(CancellationToken.None));

        actual.Should().NotContain(x => x.VCode == uitgeschrevenVereniging.VCode);
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
}


