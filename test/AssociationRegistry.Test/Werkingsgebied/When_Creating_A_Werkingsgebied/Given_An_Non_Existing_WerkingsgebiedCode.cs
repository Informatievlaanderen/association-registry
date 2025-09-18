namespace AssociationRegistry.Test.Werkingsgebied.When_Creating_A_Werkingsgebied;

using AssociationRegistry.Grar.NutsLau;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Xunit;

public class Given_An_Non_Existing_WerkingsgebiedCode
{
    [Fact]
    public async ValueTask Then_Throws_WerkingsgebiedCodeIsNietGekend()
    {
        var fixture = new Fixture().CustomizeDomain();
        var documentstore = await TestDocumentStoreFactory.CreateAsync(nameof(Given_An_Non_Existing_WerkingsgebiedCode));
        await using var session = documentstore.LightweightSession();
        var postalInfo = fixture.Create<PostalNutsLauInfo>();
        session.Store(postalInfo);
        await session.SaveChangesAsync();

        var sut = new WerkingsgebiedenService(session);
        Assert.ThrowsAny<WerkingsgebiedCodeIsNietGekend>(() => sut.Create(fixture.Create<string>()));
    }
}
