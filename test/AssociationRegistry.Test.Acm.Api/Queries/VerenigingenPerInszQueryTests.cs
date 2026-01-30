namespace AssociationRegistry.Test.Acm.Api.Queries;

using AssociationRegistry.Acm.Api.Queries.VerenigingenPerInsz;
using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using AutoFixture;
using Common.Framework;
using FluentAssertions;
using Framework;
using Marten;
using Xunit;

public class VerenigingenPerInszQueryFixture: IAsyncLifetime
{
    public DocumentStore Store { get; set; }
    public VerenigingenPerInszDocument VerenigingPerInszDocument { get; set; }
    public VerenigingenPerInszDocument VerenigingenPerInszDocumentWithoutKboVerenigingen { get; set; }

    public async ValueTask InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.CreateAsync(nameof(VerenigingenPerInszQueryTests));
        await InsertData();
    }

    private async Task InsertData()
    {
        var fixture = new Fixture().CustomizeAcmApi();

        var session = Store.LightweightSession();
        var vereniging = fixture.Create<VerenigingenPerInszDocument>();
        vereniging.Verenigingen[2].KboNummer = string.Empty;

        var vereniging2 = fixture.Create<VerenigingenPerInszDocument>();
        vereniging2.Verenigingen[2].KboNummer = string.Empty;

        session.Store(vereniging);
        session.Store(vereniging2);
        await session.SaveChangesAsync();

        VerenigingPerInszDocument = vereniging;
        VerenigingenPerInszDocumentWithoutKboVerenigingen = vereniging with
        {
            Verenigingen = vereniging.Verenigingen.Where(x => x.KboNummer == string.Empty).ToList(),
        };
    }

    public async ValueTask DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

public class VerenigingenPerInszQueryTests : IClassFixture<VerenigingenPerInszQueryFixture>, IDisposable, IAsyncDisposable
{
    private readonly VerenigingenPerInszQueryFixture _setupFixture;
    private readonly IDocumentSession _session;
    private VerenigingenPerInszQuery _query;

    public VerenigingenPerInszQueryTests(VerenigingenPerInszQueryFixture setupFixture)
    {
        _setupFixture = setupFixture;
        _session = setupFixture.Store.LightweightSession();
        _query = new VerenigingenPerInszQuery(_session);
    }

    [Fact]
    public async ValueTask With_IncludeKboVerenigingen_Returns_Kbo_Verenigingen()
    {
        var actual = await _query.ExecuteAsync(new VerenigingenPerInszFilter(_setupFixture.VerenigingPerInszDocument.Insz, IncludeKboVerenigingen: true), CancellationToken.None);

        actual.Should().BeEquivalentTo(_setupFixture.VerenigingPerInszDocument);
    }

    [Fact]
    public async ValueTask Without_KboVerenigingen_Returns_Kbo_Verenigingen()
    {
        var actual = await _query.ExecuteAsync(new VerenigingenPerInszFilter(_setupFixture.VerenigingPerInszDocument.Insz, IncludeKboVerenigingen: false), CancellationToken.None);

        actual.Should().BeEquivalentTo(_setupFixture.VerenigingenPerInszDocumentWithoutKboVerenigingen);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async ValueTask With_Other_Insz_Verenigingen_Returns_No_Verenigingen(bool includeKboVerenigingen)
    {
        var randomInsz = "123456789";

        var query = new VerenigingenPerInszQuery(_session);

        var actual = await query.ExecuteAsync(new VerenigingenPerInszFilter(randomInsz, IncludeKboVerenigingen: includeKboVerenigingen), CancellationToken.None);

        actual.Insz.Should().Be(randomInsz);
        actual.Verenigingen.Should().BeEmpty();
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
