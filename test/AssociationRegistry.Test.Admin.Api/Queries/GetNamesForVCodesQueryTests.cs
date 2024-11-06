namespace AssociationRegistry.Test.Admin.Api.Queries;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using FluentAssertions;
using Marten;
using Public.Api.Queries;
using Public.Schema.Detail;
using Vereniging;
using Xunit;
using Xunit.Categories;

public class GetNamesForVCodesQueryFixture : IAsyncLifetime
{
    public DocumentStore Store { get; set; }

    public async Task InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.Create(nameof(GetNamesForVCodesQueryTests));
    }

    public async Task DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

[IntegrationTest]
public class GetNamesForVCodesQueryTests : IClassFixture<GetNamesForVCodesQueryFixture>, IDisposable, IAsyncDisposable
{
    private readonly IDocumentSession _session;

    public GetNamesForVCodesQueryTests(GetNamesForVCodesQueryFixture setupFixture)
    {
        _session = setupFixture.Store.LightweightSession();
    }

    [Fact]
    public async Task Returns_Empty_Collection_When_No_VCodes_Provided()
    {
        var query = new GetNamesForVCodesQuery(_session);

        var actual = await query.ExecuteAsync(new GetNamesForVCodesFilter(), CancellationToken.None);

        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task Returns_Empty_Collection_When_No_VCodes_Matched()
    {
        var fixture = new Fixture().CustomizeDomain();
        var andereVCode = fixture.Create<VCode>();

        var vereniging = fixture.Create<BeheerVerenigingDetailDocument>();
        _session.Store(vereniging);
        await _session.SaveChangesAsync();

        var query = new GetNamesForVCodesQuery(_session);

        var actual = await query.ExecuteAsync(new GetNamesForVCodesFilter(andereVCode), CancellationToken.None);

        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task Returns_Collection_Of_VCode_With_Names_When_All_VCodes_Match()
    {
        var fixture = new Fixture().CustomizeDomain();

        var verenigingen = fixture.CreateMany<BeheerVerenigingDetailDocument>()
                                  .ToList();

        _session.StoreObjects(verenigingen);
        await _session.SaveChangesAsync();

        var vCodesAndNaam = verenigingen.ToDictionary(x => x.VCode, x => x.Naam);
        var vCodes = vCodesAndNaam.Keys.ToArray();

        var query = new GetNamesForVCodesQuery(_session);

        var actual = await query.ExecuteAsync(new GetNamesForVCodesFilter(vCodes), CancellationToken.None);

        actual.Should().BeEquivalentTo(vCodesAndNaam);
    }

    [Fact]
    public async Task Returns_Collection_Of_VCode_With_Names_When_Some_VCodes_Are_Not_Found_In_Projection()
    {
        var fixture = new Fixture().CustomizeDomain();

        var verenigingen = fixture.CreateMany<BeheerVerenigingDetailDocument>()
                                  .ToList();

        _session.StoreObjects(verenigingen);
        await _session.SaveChangesAsync();

        var vCodesAndNaam = verenigingen.ToDictionary(x => x.VCode, x => x.Naam);
        var vCodes = vCodesAndNaam.Keys
                                  .Append(fixture.Create<VCode>())
                                  .ToArray();

        var query = new GetNamesForVCodesQuery(_session);

        var actual = await query.ExecuteAsync(new GetNamesForVCodesFilter(vCodes), CancellationToken.None);

        actual.Should().BeEquivalentTo(vCodesAndNaam);
    }

    [Fact]
    public async Task Returns_Only_Matching_Collection_Of_VCode_With_Names()
    {
        var fixture = new Fixture().CustomizeDomain();

        var verenigingen = fixture.CreateMany<BeheerVerenigingDetailDocument>()
                                  .ToList();

        _session.StoreObjects(verenigingen);
        await _session.SaveChangesAsync();

        var vCodesAndNaam = verenigingen.ToDictionary(x => x.VCode, x => x.Naam);
        var singleVCode = vCodesAndNaam.Keys.First();

        var query = new GetNamesForVCodesQuery(_session);

        var actual = await query.ExecuteAsync(new GetNamesForVCodesFilter(singleVCode), CancellationToken.None);

        actual.Should().BeEquivalentTo(new Dictionary<string, string>()
        {
            { singleVCode, vCodesAndNaam[singleVCode] },
        });
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