namespace AssociationRegistry.Test.Werkingsgebied.When_Creating_A_Werkingsgebied;

using AssociationRegistry.Grar.NutsLau;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_An_Existing_WerkingsgebiedCode
{
    [Fact]
    public async ValueTask Then_We_Create_A_Werkingsgebied()
    {
        var fixture = new Fixture().CustomizeDomain();
        var documentstore = await TestDocumentStoreFactory.CreateAsync(nameof(Given_An_Existing_WerkingsgebiedCode));
        await using var session = documentstore.LightweightSession();
        var postalInfo = fixture.Create<PostalNutsLauInfo>();
        var werkingsgebiedCode = $"{postalInfo.Nuts3}{postalInfo.Lau}";

        session.Store(postalInfo);
        await session.SaveChangesAsync();

        var sut = new WerkingsgebiedenService(session);
        var actual = sut.Create(werkingsgebiedCode);
        actual.Code.Should().BeEquivalentTo(werkingsgebiedCode);
        actual.Naam.Should().BeEquivalentTo(postalInfo.Gemeentenaam);
    }
}
