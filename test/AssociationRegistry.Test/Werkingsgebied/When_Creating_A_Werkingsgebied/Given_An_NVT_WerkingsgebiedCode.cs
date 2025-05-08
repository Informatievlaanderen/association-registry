namespace AssociationRegistry.Test.Werkingsgebied.When_Creating_A_Werkingsgebied;

using AssociationRegistry.Grar.NutsLau;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using FluentAssertions;
using Vereniging;
using Xunit;

public class Given_An_NVT_WerkingsgebiedCode
{
    [Fact]
    public async ValueTask Then_We_Create_A_Werkingsgebied()
    {
        var fixture = new Fixture().CustomizeDomain();
        var documentstore = await TestDocumentStoreFactory.CreateAsync(nameof(Given_An_NVT_WerkingsgebiedCode));
        await using var session = documentstore.LightweightSession();
        var postalInfo = fixture.Create<PostalNutsLauInfo>();
        session.Store(postalInfo);
        await session.SaveChangesAsync();

        var sut = new WerkingsgebiedenService(session);
        var actual = sut.Create(Werkingsgebied.NietVanToepassing.Code);
        actual.Should().BeEquivalentTo(Werkingsgebied.NietVanToepassing);
    }
}
