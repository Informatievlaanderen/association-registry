namespace AssociationRegistry.Test.Admin.Api.Queries.FilterVzerOnlyQueryTests;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using CommandHandling.MagdaSync.SyncKsz.Queries;
using Events;
using FluentAssertions;
using Marten;
using Xunit;

public class FilterVzerOnlyQueryFixture : IAsyncLifetime
{
    public DocumentStore Store { get; set; }

    public async ValueTask InitializeAsync()
    {
        Store = await TestDocumentStoreFactory.CreateAsync(nameof(FilterVzerOnlyQueryTests));
    }

    public async ValueTask DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

public class FilterVzerOnlyQueryTests : IClassFixture<FilterVzerOnlyQueryFixture>, IDisposable, IAsyncDisposable
{
    private readonly IDocumentSession _session;

    public FilterVzerOnlyQueryTests(FilterVzerOnlyQueryFixture setupFixture)
    {
        _session = setupFixture.Store.LightweightSession();
    }

    [Fact]
    public async ValueTask Returns_Empty_Collection_When_No_VCodes_Provided()
    {
        var query = new FilterVzerOnlyQuery(_session);

        var actual = await query.ExecuteAsync(new FilterVzerOnlyQueryFilter([]), CancellationToken.None);

        actual.Should().BeEmpty();
    }

    [Fact]
    public async ValueTask Returns_Only_VCodes_That_Are_Vzer_Or_GemigreerdeFVs()
    {

        var fixture = new Fixture().CustomizeDomain();

        _session.Events.Append("V0001001", fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with {VCode = "V0001001"});
        _session.Events.Append(
            "V0001002", fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens>() with { VCode = "V0001002"});
        _session.Events.Append(
            "V0001003", fixture.Create<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid>() with {VCode = "V0001003"});

        await _session.SaveChangesAsync();

        var query = new FilterVzerOnlyQuery(_session);

        var actual = await query.ExecuteAsync(new FilterVzerOnlyQueryFilter([
            VCode.Hydrate("V0001001"),
            VCode.Hydrate("V0001002"),
            VCode.Hydrate("V0001003"),
        ]), CancellationToken.None);

        actual.Should().BeEquivalentTo([
            VCode.Hydrate("V0001002"),
            VCode.Hydrate("V0001003")]);
    }

    [Fact]
    public async ValueTask Returns_Only_Existing_Verenigingen()
    {

        var fixture = new Fixture().CustomizeDomain();

        _session.Events.Append(
            "V0001004", fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = "V0001004" });
        _session.Events.Append(
            "V0001005", fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens>() with {VCode = "V0001005"} );

        await _session.SaveChangesAsync();

        var query = new FilterVzerOnlyQuery(_session);

        var andereVCode = VCode.Hydrate("V0001003");
        var actual = await query.ExecuteAsync(new FilterVzerOnlyQueryFilter([andereVCode]), CancellationToken.None);

        actual.Should().BeEmpty();
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
