namespace AssociationRegistry.Test.Acm.Api.Queries;

using AssociationRegistry.Acm.Api.Queries.VerenigingenPerInsz;
using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using AutoFixture;
using Common.Framework;
using FluentAssertions;
using Framework;
using Marten;
using Xunit;
using Xunit.Categories;

public class VerenigingenPerInszQueryFixture: IAsyncLifetime
{
    public DocumentStore Store { get; set; }

    public async Task InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.Create(nameof(VerenigingenPerInszQueryTests));
    }

    public async Task DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

[IntegrationTest]
public class VerenigingenPerInszQueryTests : IClassFixture<VerenigingenPerInszQueryFixture>, IDisposable, IAsyncDisposable
{
    private readonly IDocumentSession _session;

    public VerenigingenPerInszQueryTests(VerenigingenPerInszQueryFixture setupFixture)
    {
        _session = setupFixture.Store.LightweightSession();
    }

    [Fact]
    public async Task Returns_VerenigingenPerInszDocument()
    {
        var vereniging = await StoreVereniging(_session);

        var query = new VerenigingenPerInszQuery(_session);

        var actual = await query.ExecuteAsync(new VerenigingenPerInszFilter(vereniging.Insz), CancellationToken.None);

        actual.Should().BeEquivalentTo(vereniging);
    }

    [Fact]
    public async Task With_Multiple_Verenigingen_Returns_Correct_VerenigingenPerInszDocument()
    {
        var vereniging = await StoreVereniging(_session);
        await StoreVereniging(_session);

        var query = new VerenigingenPerInszQuery(_session);

        var actual = await query.ExecuteAsync(new VerenigingenPerInszFilter(vereniging.Insz), CancellationToken.None);

        actual.Should().BeEquivalentTo(vereniging);
    }

    [Fact]
    public async Task With_Other_Insz_Verenigingen_Returns_No_Verenigingen()
    {
        var insz = "123456789";
        await StoreVereniging(_session);
        await StoreVereniging(_session);

        var query = new VerenigingenPerInszQuery(_session);

        var actual = await query.ExecuteAsync(new VerenigingenPerInszFilter(insz), CancellationToken.None);

        actual.Insz.Should().Be(insz);
        actual.Verenigingen.Should().BeEmpty();
    }

    private async Task<VerenigingenPerInszDocument> StoreVereniging(IDocumentSession session)
    {
        var fixture = new Fixture().CustomizeAcmApi();

        var vereniging = fixture.Create<VerenigingenPerInszDocument>();

        session.Store(vereniging);
        await session.SaveChangesAsync();

        return vereniging;
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
