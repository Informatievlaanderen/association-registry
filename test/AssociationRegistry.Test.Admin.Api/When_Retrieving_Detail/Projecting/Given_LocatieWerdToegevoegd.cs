namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using Events;
using AutoFixture;
using FluentAssertions;
using Formatters;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_LocatieWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieWerdToegevoegd = new TestEvent<LocatieWerdToegevoegd>(fixture.Create<LocatieWerdToegevoegd>());

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);

        doc.Locaties.Should().HaveCount(4);
        doc.Locaties.Should().ContainEquivalentOf(
            new BeheerVerenigingDetailDocument.Locatie
            {
                LocatieId = locatieWerdToegevoegd.Data.Locatie.LocatieId,
                IsPrimair = locatieWerdToegevoegd.Data.Locatie.IsPrimair,
                Naam = locatieWerdToegevoegd.Data.Locatie.Naam,
                Locatietype = locatieWerdToegevoegd.Data.Locatie.Locatietype,
                Adres = locatieWerdToegevoegd.Data.Locatie.Adres is null
                    ? null
                    : new Adres
                    {
                        Straatnaam = locatieWerdToegevoegd.Data.Locatie.Adres.Straatnaam,
                        Huisnummer = locatieWerdToegevoegd.Data.Locatie.Adres.Huisnummer,
                        Busnummer = locatieWerdToegevoegd.Data.Locatie.Adres.Busnummer,
                        Postcode = locatieWerdToegevoegd.Data.Locatie.Adres.Postcode,
                        Gemeente = locatieWerdToegevoegd.Data.Locatie.Adres.Gemeente,
                        Land = locatieWerdToegevoegd.Data.Locatie.Adres.Land,
                    },
                Adresvoorstelling = locatieWerdToegevoegd.Data.Locatie.Adres.ToAdresString(),
                AdresId = locatieWerdToegevoegd.Data.Locatie.AdresId is null
                    ? null
                    : new AdresId
                    {
                        Broncode = locatieWerdToegevoegd.Data.Locatie.AdresId?.Broncode,
                        Bronwaarde = locatieWerdToegevoegd.Data.Locatie.AdresId?.Bronwaarde,
                    },
            });
    }
}
