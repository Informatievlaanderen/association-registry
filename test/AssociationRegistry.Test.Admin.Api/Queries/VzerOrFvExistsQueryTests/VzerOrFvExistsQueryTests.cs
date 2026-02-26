namespace AssociationRegistry.Test.Admin.Api.Queries.VzerOrFvExistsQueryTests;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Events;
using FluentAssertions;
using Marten;
using Xunit;

public class VzerOrFvExistsQueryTestsFixture : IAsyncLifetime
{
    public const string VCodeRechtspersoon = "V0001001";
    public const string VCodeVzer = "V0001002";
    public const string VCodeFv = "V0001003";
    public DocumentStore Store { get; set; }
    public Fixture _fixture;

    public async ValueTask InitializeAsync()
    {
        _fixture = new Fixture().CustomizeDomain();

        Store = await TestDocumentStoreFactory.CreateAsync(nameof(VzerOrFvExistsQueryTestsTests));
        var session = Store.LightweightSession();

        session.Events.Append(
            VCodeRechtspersoon,
            _fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCodeRechtspersoon,
            }
        );
        session.Events.Append(
            VCodeVzer,
            _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens>() with
            {
                VCode = VCodeVzer,
            }
        );
        session.Events.Append(
            VCodeFv,
            _fixture.Create<FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens>() with
            {
                VCode = VCodeFv,
            }
        );

        await session.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Store.DisposeAsync();
    }
}

public class VzerOrFvExistsQueryTestsTests
    : IClassFixture<VzerOrFvExistsQueryTestsFixture>,
        IDisposable,
        IAsyncDisposable
{
    private readonly IDocumentSession _session;
    private readonly Fixture _fixture;

    public VzerOrFvExistsQueryTestsTests(VzerOrFvExistsQueryTestsFixture setupFixture)
    {
        _session = setupFixture.Store.LightweightSession();
        _fixture = setupFixture._fixture;
    }

    [Fact]
    public async ValueTask With_Different_VCode_Then_Vzer_Or_Fv_Registration_Then_Returns_False()
    {
        var query = new VzerOrFvExistsQuery(_session);

        var actual = await query.ExecuteAsync(
            new VzerOrFvExistsFilter(_fixture.Create<VCode>().Value),
            CancellationToken.None
        );

        actual.Should().BeFalse();
    }

    [Theory]
    [InlineData(VzerOrFvExistsQueryTestsFixture.VCodeVzer)]
    [InlineData(VzerOrFvExistsQueryTestsFixture.VCodeFv)]
    public async ValueTask With_Matching_Vzer_Or_FV_VCode_Then_Returns_True(string vCode)
    {
        var query = new VzerOrFvExistsQuery(_session);

        var actual = await query.ExecuteAsync(new VzerOrFvExistsFilter(vCode), CancellationToken.None);

        actual.Should().BeTrue();
    }

    [Fact]
    public async ValueTask With_RechtspersoonVCode_Then_Return_False()
    {
        var query = new VzerOrFvExistsQuery(_session);

        var actual = await query.ExecuteAsync(
            new VzerOrFvExistsFilter(VzerOrFvExistsQueryTestsFixture.VCodeRechtspersoon),
            CancellationToken.None
        );

        actual.Should().BeFalse();
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
