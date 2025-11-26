namespace AssociationRegistry.Test.Admin.Api.Queries.BewaartermijnQuery;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Schema.Bewaartermijn;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using FluentAssertions;
using Marten;
using System.ComponentModel;
using Xunit;

public class BewaartermijnQueryFixture : IAsyncLifetime
{
    public DocumentStore Store { get; set; }

    public async ValueTask InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.CreateAsync(nameof(BewaartermijnQueryTests));
    }

    public async ValueTask DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

public class BewaartermijnQueryTests : IClassFixture<BewaartermijnQueryFixture>, IDisposable, IAsyncDisposable
{
    private readonly IDocumentSession _session;

    public BewaartermijnQueryTests(BewaartermijnQueryFixture setupFixture)
    {
        _session = setupFixture.Store.LightweightSession();
    }

    [Fact]
    public async ValueTask Returns_Null_When_No_BewaartermijnId_Provided()
    {
        var query = new BewaartermijnQuery(_session);

        var actual = await query.ExecuteAsync(new BewaartermijnFilter(""), CancellationToken.None);

        actual.Should().BeNull();
    }

    [Fact]
    public async ValueTask Throw_When_Null_Bewaartermijn_Provided()
    {
        var query = new BewaartermijnQuery(_session);

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await query.ExecuteAsync(new BewaartermijnFilter(null), CancellationToken.None));
    }

    [Fact]
    public async ValueTask Returns_Detail_When_Bewaartermijn_Matches()
    {
        var fixture = new Fixture().CustomizeDomain();

        var bewaartermijnen = fixture.CreateMany<BewaartermijnDocument>()
                                  .ToList();

        _session.StoreObjects(bewaartermijnen);
        await _session.SaveChangesAsync();

        var query = new BewaartermijnQuery(_session);

        var actual = await query.ExecuteAsync(new BewaartermijnFilter(bewaartermijnen[1].BewaartermijnId), CancellationToken.None);

        actual.Should().BeEquivalentTo(bewaartermijnen[1]);
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
