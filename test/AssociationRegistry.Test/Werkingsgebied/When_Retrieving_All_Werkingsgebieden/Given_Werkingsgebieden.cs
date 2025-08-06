namespace AssociationRegistry.Test.Werkingsgebied.When_Retrieving_All_Werkingsgebieden;

using AssociationRegistry.Grar.NutsLau;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Vereniging;
using Xunit;

public class Given_Werkingsgebieden
{
    IReadOnlyList<Werkingsgebied> _actual;
    public Given_Werkingsgebieden()
    {
        var fixture = new Fixture().CustomizeDomain();
        var documentstore = TestDocumentStoreFactory.CreateAsync(nameof(Given_Werkingsgebieden)).GetAwaiter().GetResult();
        using var session = documentstore.LightweightSession();
        var postalInfo = fixture.Create<PostalNutsLauInfo>();

        session.Store(postalInfo);
        session.SaveChangesAsync().GetAwaiter().GetResult();

        var sut = new WerkingsgebiedenService(session);
        _actual = sut.AllWithNVT();
    }

    [Fact]
    public async ValueTask Then_Provincies_Are_In_The_List()
        => _actual.Should().Contain(WellKnownWerkingsgebieden.Provincies);

    [Fact]
    public async ValueTask Then_NVT_Is_In_The_List()
        => _actual.Should().Contain(Werkingsgebied.NietVanToepassing);
}
