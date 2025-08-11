namespace AssociationRegistry.Test.Projections.Beheer.Detail.Registratie;

using Admin.Schema;
using Admin.Schema.Detail;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging;
using Events;
using Scenario.Registratie;
using Vereniging;
using Vereniging.Bronnen;
using Contactgegeven = Admin.Schema.Detail.Contactgegeven;
using Doelgroep = Admin.Schema.Detail.Doelgroep;
using HoofdactiviteitVerenigingsloket = Admin.Schema.Detail.HoofdactiviteitVerenigingsloket;
using Locatie = Admin.Schema.Detail.Locatie;
using Verenigingstype = Admin.Schema.Detail.Verenigingstype;
using Vertegenwoordiger = Admin.Schema.Detail.Vertegenwoordiger;
using WellknownFormats = Admin.ProjectionHost.Constants.WellknownFormats;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
    BeheerDetailScenarioFixture<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario> fixture)
    : BeheerDetailScenarioClassFixture<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(1);

    [Fact]
    public void Document_Is_Updated()
    {
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
        var expected = MapVereniging(feitelijkeVerenigingWerdGeregistreerd);
        fixture.Result.Should().BeEquivalentTo(expected);
    }

    private BeheerVerenigingDetailDocument MapVereniging(
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
    {
        return new BeheerVerenigingDetailDocument
        {
            JsonLdMetadataType = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
            VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            Verenigingstype = new Verenigingstype
            {
                Code = DecentraalBeheer.Vereniging.Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm).Code,
                Naam = DecentraalBeheer.Vereniging.Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm).Naam,
            },
            Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam,
            Roepnaam = string.Empty,
            KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam,
            KorteBeschrijving = string.Empty,
            Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum?.ToString(WellknownFormats.DateOnly),
            Doelgroep = new Doelgroep
            {
                JsonLdMetadata = new JsonLdMetadata
                {
                    Id = JsonLdType.Doelgroep.CreateWithIdValues(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
                    Type = JsonLdType.Doelgroep.Type,
                },
                Minimumleeftijd = DecentraalBeheer.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                Maximumleeftijd = DecentraalBeheer.Vereniging.Doelgroep.StandaardMaximumleeftijd,
            },
            Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm,
            DatumLaatsteAanpassing = "1970-01-01",
            Status = VerenigingStatus.Actief.StatusNaam,
            Contactgegevens = Array.Empty<Contactgegeven>(),
            Locaties = Array.Empty<Locatie>(),
            Vertegenwoordigers = Array.Empty<Vertegenwoordiger>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<HoofdactiviteitVerenigingsloket>(),
            Sleutels = new Sleutel[]
            {
                new()
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.Sleutel.CreateWithIdValues(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                                                                   Sleutelbron.VR.Waarde),
                        Type = JsonLdType.Sleutel.Type,
                    },
                    Bron = Sleutelbron.VR.Waarde,
                    Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                    CodeerSysteem = CodeerSysteem.VR.Waarde,
                    GestructureerdeIdentificator = new GestructureerdeIdentificator
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode, Sleutelbron.VR.Waarde),
                            Type = JsonLdType.GestructureerdeSleutel.Type,
                        },
                        Nummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                    },
                },
                new()
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.Sleutel.CreateWithIdValues(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                                                                   Sleutelbron.KBO.Waarde),
                        Type = JsonLdType.Sleutel.Type,
                    },
                    Bron = Sleutelbron.KBO.Waarde,
                    Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer,
                    CodeerSysteem = CodeerSysteem.KBO.Waarde,
                    GestructureerdeIdentificator = new GestructureerdeIdentificator
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode, Sleutelbron.KBO.Waarde),
                            Type = JsonLdType.GestructureerdeSleutel.Type,
                        },
                        Nummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer,
                    },
                },
            },
            Relaties = Array.Empty<Relatie>(),
            Bron = Bron.KBO,
            Metadata = new Metadata(1, 1),
        };
    }
}
