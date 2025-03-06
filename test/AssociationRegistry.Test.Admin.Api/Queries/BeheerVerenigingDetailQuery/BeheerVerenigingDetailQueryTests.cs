namespace AssociationRegistry.Test.Admin.Api.Queries.BeheerVerenigingDetailQuery;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using FluentAssertions;
using Marten;
using Xunit;
using Xunit.Categories;

public class BeheerVerenigingDetailQueryFixture : IAsyncLifetime
{
    public DocumentStore Store { get; set; }

    public async ValueTask InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.CreateAsync(nameof(BeheerVerenigingDetailQueryTests));
    }

    public async ValueTask DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

[IntegrationTest]
[Category(Categories.ReplaceThisQueryWithGetNamesForVCodeForOneUsage)]
public class BeheerVerenigingDetailQueryTests : IClassFixture<BeheerVerenigingDetailQueryFixture>, IDisposable, IAsyncDisposable
{
    private readonly IDocumentSession _session;

    public BeheerVerenigingDetailQueryTests(BeheerVerenigingDetailQueryFixture setupFixture)
    {
        _session = setupFixture.Store.LightweightSession();
    }

    [Fact]
    public async Task Returns_Null_When_No_VCode_Provided()
    {
        var query = new BeheerVerenigingDetailQuery(_session);

        var actual = await query.ExecuteAsync(new BeheerVerenigingDetailFilter(""), CancellationToken.None);

        actual.Should().BeNull();
    }

    [Fact]
    public async Task Throw_When_Null_VCode_Provided()
    {
        var query = new BeheerVerenigingDetailQuery(_session);

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await query.ExecuteAsync(new BeheerVerenigingDetailFilter(null), CancellationToken.None));
    }

    [Fact]
    public async Task Returns_Detail_When_VCode_Matches()
    {
        var fixture = new Fixture().CustomizeDomain();

        var verenigingen = fixture.CreateMany<BeheerVerenigingDetailDocument>()
                                  .ToList();

        _session.StoreObjects(verenigingen);
        await _session.SaveChangesAsync();

        var query = new BeheerVerenigingDetailQuery(_session);

        var actual = await query.ExecuteAsync(new BeheerVerenigingDetailFilter(verenigingen[1].VCode), CancellationToken.None);

        actual.Should().BeEquivalentTo(verenigingen[1]);
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
