namespace AssociationRegistry.Test.Werkingsgebied.When_Retrieving_All_Werkingsgebieden;

using AssociationRegistry.Grar.NutsLau;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Marten;
using Vereniging;
using Xunit;

public class Given_Werkingsgebieden : IAsyncLifetime
{
    private IDocumentSession _session = null!;
    private IReadOnlyList<Werkingsgebied> _actual = null!;

    public async ValueTask InitializeAsync()
    {
        var fixture = new Fixture().CustomizeDomain();
        var documentstore = await TestDocumentStoreFactory.CreateAsync(nameof(Given_Werkingsgebieden));
        _session = documentstore.LightweightSession();

        _session.Store(fixture.Create<PostalNutsLauInfo>());
        await _session.SaveChangesAsync();

        _actual = await new WerkingsgebiedenService(_session).AllWithNVT();
    }

    public ValueTask DisposeAsync() => _session.DisposeAsync();

    [Fact]
    public void Then_Provincies_Are_In_The_List() => _actual.Should().Contain(WellKnownWerkingsgebieden.Provincies);

    [Fact]
    public void Then_NVT_Is_In_The_List() => _actual.Should().Contain(Werkingsgebied.NietVanToepassing);
}
