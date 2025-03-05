namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using Admin.Schema.Constants;
using AssociationRegistry.Framework;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Formats;
using Framework;
using JsonLdContext;
using Vereniging;

using Xunit;
using Xunit.Categories;
using Doelgroep = AssociationRegistry.Public.Schema.Detail.Doelgroep;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;

[UnitTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_vereniging()
    {
        var fixture = new Fixture().CustomizePublicApi();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            new TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>(
                fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>());

        var doc = PubliekVerenigingDetailProjector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new PubliekVerenigingDetailDocument
            {
                VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                JsonLdMetadataType = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
                Verenigingstype = new PubliekVerenigingDetailDocument.VerenigingsType
                {
                    Code = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Code,
                    Naam = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Naam,
                },
                Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
                Roepnaam = string.Empty,
                KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = string.Empty,
                Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum,
                Doelgroep = new Doelgroep
                {
                    JsonLdMetadata =
                        new JsonLdMetadata(
                            JsonLdType.Doelgroep.CreateWithIdValues(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode),
                            JsonLdType.Doelgroep.Type),
                    Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                    Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
                },
                Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
                DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                   .FormatAsBelgianDate(),
                Status = VerenigingStatus.Actief,
                IsUitgeschrevenUitPubliekeDatastroom = false,
                Contactgegevens = Array.Empty<PubliekVerenigingDetailDocument.Contactgegeven>(),
                Locaties = Array.Empty<PubliekVerenigingDetailDocument.Locatie>(),
                HoofdactiviteitenVerenigingsloket = Array.Empty<PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket>(),
                Sleutels = new PubliekVerenigingDetailDocument.Sleutel[]
                {
                    new()
                    {
                        JsonLdMetadata = new JsonLdMetadata(
                            JsonLdType.Sleutel.CreateWithIdValues(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                                                                  Sleutelbron.VR.Waarde),
                            JsonLdType.Sleutel.Type),
                        Bron = Sleutelbron.VR.Waarde,
                        GestructureerdeIdentificator = new PubliekVerenigingDetailDocument.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = new JsonLdMetadata(
                                JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                    verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                                    Sleutelbron.VR.Waarde),
                                JsonLdType.GestructureerdeSleutel.Type),
                            Nummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                        },
                        Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                        CodeerSysteem = CodeerSysteem.VR,
                    },
                    new()
                    {
                        JsonLdMetadata = new JsonLdMetadata(
                            JsonLdType.Sleutel.CreateWithIdValues(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                                                                  Sleutelbron.KBO.Waarde),
                            JsonLdType.Sleutel.Type),
                        GestructureerdeIdentificator = new PubliekVerenigingDetailDocument.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = new JsonLdMetadata(
                                JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                    verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode, Sleutelbron.KBO.Waarde),
                                JsonLdType.GestructureerdeSleutel.Type),
                            Nummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                        },
                        Bron = Sleutelbron.KBO.Waarde,
                        Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                        CodeerSysteem = CodeerSysteem.KBO,
                    },
                },
            });
    }
}
