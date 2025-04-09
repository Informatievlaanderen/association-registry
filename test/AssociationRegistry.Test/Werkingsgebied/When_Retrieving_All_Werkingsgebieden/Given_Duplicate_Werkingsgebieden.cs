namespace AssociationRegistry.Test.Werkingsgebied.When_Retrieving_All_Werkingsgebieden;

using AssociationRegistry.Grar.NutsLau;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using FluentAssertions;
using Vereniging;
using Xunit;

public class Given_Duplicate_Werkingsgebieden
{
    [Fact]
    public async Task Then_We_Only_Return_One_Werkingsgebied()
    {
        var fixture = new Fixture().CustomizeDomain();
        var documentstore = await TestDocumentStoreFactory.CreateAsync(nameof(When_Creating_A_Werkingsgebied));
        await using var session = documentstore.LightweightSession();
        var postalInfo = fixture.Create<PostalNutsLauInfo>();
        var duplicateWerkingsgebied = fixture.Create<PostalNutsLauInfo>() with
        {
            Postcode = fixture.Create<string>(),
        };
        var werkingsgebiedCode = $"{postalInfo.Nuts}{postalInfo.Lau}";

        session.Store(postalInfo, duplicateWerkingsgebied);
        await session.SaveChangesAsync();

        var sut = new WerkingsgebiedenService(session);
        var actual = sut.AllWithNVT();
        actual.Count(x => x.Code == werkingsgebiedCode).Should().Be(1);
    }
}
