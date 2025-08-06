namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using Framework;
using JsonLdContext;
using Vereniging;
using Xunit;

public class Given_MaatschappelijkeZetelWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var maatschappelijkeZetelVolgensKboWerdGewijzigd = fixture.Create<TestEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();
        doc.VCode = fixture.Create<VCode>();

        var locatie = fixture.Create<PubliekVerenigingDetailDocument.Types.Locatie>() with
        {
            LocatieId = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.LocatieId,
            JsonLdMetadata =
            new JsonLdMetadata(
                JsonLdType.Locatie.CreateWithIdValues(doc.VCode,
                                                      maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.LocatieId.ToString()),
                JsonLdType.Locatie.Type),
        };

        doc.Locaties = doc.Locaties.Append(locatie).ToArray();

        PubliekVerenigingDetailProjector.Apply(maatschappelijkeZetelVolgensKboWerdGewijzigd, doc);

        doc.Locaties.Should().HaveCount(4);

        doc.Locaties.Should().ContainEquivalentOf(
            new PubliekVerenigingDetailDocument.Types.Locatie
            {
                JsonLdMetadata =
                    new JsonLdMetadata(
                        JsonLdType.Locatie.CreateWithIdValues(doc.VCode,
                                                              maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.LocatieId.ToString()),
                        JsonLdType.Locatie.Type),
                LocatieId = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.LocatieId,
                IsPrimair = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.IsPrimair,
                Naam = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.Naam,
                Locatietype = locatie.Locatietype,
                Adres = locatie.Adres,
                Adresvoorstelling = locatie.Adresvoorstelling,
                AdresId = locatie.AdresId,
                VerwijstNaar = locatie.VerwijstNaar,
            });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
