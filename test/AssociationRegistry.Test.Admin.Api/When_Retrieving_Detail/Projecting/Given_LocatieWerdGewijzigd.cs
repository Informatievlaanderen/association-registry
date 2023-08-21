namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Formatters;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_LocatieWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieWerdGewijzigd = fixture.Create<TestEvent<LocatieWerdGewijzigd>>();

        var locatie = fixture.Create<BeheerVerenigingDetailDocument.Locatie>() with
        {
            LocatieId = locatieWerdGewijzigd.Data.Locatie.LocatieId,
        };

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();
        doc.Locaties = doc.Locaties.Append(locatie).ToArray();

        BeheerVerenigingDetailProjector.Apply(locatieWerdGewijzigd, doc);

        doc.Locaties.Should().HaveCount(4);
        doc.Locaties.Should().ContainEquivalentOf(
            new BeheerVerenigingDetailDocument.Locatie
            {
                LocatieId = locatieWerdGewijzigd.Data.Locatie.LocatieId,
                IsPrimair = locatieWerdGewijzigd.Data.Locatie.IsPrimair,
                Naam = locatieWerdGewijzigd.Data.Locatie.Naam,
                Locatietype = locatieWerdGewijzigd.Data.Locatie.Locatietype,
                Adres = locatieWerdGewijzigd.Data.Locatie.Adres is null
                    ? null
                    : new Adres
                    {
                        Straatnaam = locatieWerdGewijzigd.Data.Locatie.Adres.Straatnaam,
                        Huisnummer = locatieWerdGewijzigd.Data.Locatie.Adres.Huisnummer,
                        Busnummer = locatieWerdGewijzigd.Data.Locatie.Adres.Busnummer,
                        Postcode = locatieWerdGewijzigd.Data.Locatie.Adres.Postcode,
                        Gemeente = locatieWerdGewijzigd.Data.Locatie.Adres.Gemeente,
                        Land = locatieWerdGewijzigd.Data.Locatie.Adres.Land,
                    },
                Adresvoorstelling = locatieWerdGewijzigd.Data.Locatie.Adres.ToAdresString(),
                AdresId = locatieWerdGewijzigd.Data.Locatie.AdresId is null
                    ? null
                    : new AdresId
                    {
                        Broncode = locatieWerdGewijzigd.Data.Locatie.AdresId?.Broncode,
                        Bronwaarde = locatieWerdGewijzigd.Data.Locatie.AdresId?.Bronwaarde,
                    },
                Bron = locatie.Bron,
            });
        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
        doc.DatumLaatsteAanpassing.Should().Be(locatieWerdGewijzigd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(locatieWerdGewijzigd.Sequence, locatieWerdGewijzigd.Version));
    }
}
