namespace AssociationRegistry.Test.Projections.Beheer.Detail.Registratie;

using Admin.Schema;
using Admin.Schema.Detail;
using AssociationRegistry.Test.Projections.Scenario.Registratie;
using Events;
using Formats;
using JsonLdContext;
using Vereniging;
using Vereniging.Bronnen;
using Adres = Admin.Schema.Detail.Adres;
using AdresId = Admin.Schema.Detail.AdresId;
using Contactgegeven = Admin.Schema.Detail.Contactgegeven;
using Doelgroep = Admin.Schema.Detail.Doelgroep;
using HoofdactiviteitVerenigingsloket = Admin.Schema.Detail.HoofdactiviteitVerenigingsloket;
using Locatie = Admin.Schema.Detail.Locatie;
using Verenigingstype = Admin.Schema.Detail.Verenigingstype;
using Vertegenwoordiger = Admin.Schema.Detail.Vertegenwoordiger;
using WellknownFormats = Admin.ProjectionHost.Constants.WellknownFormats;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGeregistreerd(
    BeheerDetailScenarioFixture<FeitelijkeVerenigingWerdGeregistreerdScenario> fixture)
    : BeheerDetailScenarioClassFixture<FeitelijkeVerenigingWerdGeregistreerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(1);

    [Fact]
    public void Document_Is_Updated()
    {
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd;
        var expected = MapVereniging(feitelijkeVerenigingWerdGeregistreerd);
        fixture.Result.Should().BeEquivalentTo(expected);
    }

    private BeheerVerenigingDetailDocument MapVereniging(FeitelijkeVerenigingWerdGeregistreerd feitelijkeVerenigingWerdGeregistreerd)
    {
        return new BeheerVerenigingDetailDocument
        {
            JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type,
            VCode = feitelijkeVerenigingWerdGeregistreerd.VCode,
            Verenigingstype = new Verenigingstype
            {
                Code = AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                Naam = AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = feitelijkeVerenigingWerdGeregistreerd.Naam,
            KorteNaam = feitelijkeVerenigingWerdGeregistreerd.KorteNaam,
            KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.KorteBeschrijving,
            Startdatum = feitelijkeVerenigingWerdGeregistreerd.Startdatum?.ToString(WellknownFormats.DateOnly),
            Doelgroep = new Doelgroep
            {
                JsonLdMetadata = new JsonLdMetadata
                {
                    Id = JsonLdType.Doelgroep.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.VCode),
                    Type = JsonLdType.Doelgroep.Type,
                },
                Minimumleeftijd = feitelijkeVerenigingWerdGeregistreerd.Doelgroep.Minimumleeftijd,
                Maximumleeftijd = feitelijkeVerenigingWerdGeregistreerd.Doelgroep.Maximumleeftijd,
            },
            DatumLaatsteAanpassing = "1970-01-01",
            Status = VerenigingStatus.Actief.StatusNaam,
            Contactgegevens = feitelijkeVerenigingWerdGeregistreerd.Contactgegevens.Select(
                c => new Contactgegeven
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.Contactgegeven.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.VCode, c.ContactgegevenId.ToString()),
                        Type = JsonLdType.Contactgegeven.Type,
                    },
                    ContactgegevenId = c.ContactgegevenId,
                    Contactgegeventype = c.Contactgegeventype.ToString(),
                    Waarde = c.Waarde,
                    Beschrijving = c.Beschrijving,
                    IsPrimair = c.IsPrimair,
                    Bron = Bron.Initiator,
                }).ToArray(),
            Locaties = feitelijkeVerenigingWerdGeregistreerd.Locaties.Select(
                loc => new Locatie
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.Locatie.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.VCode, loc.LocatieId.ToString()),
                        Type = JsonLdType.Locatie.Type,
                    },
                    LocatieId = loc.LocatieId,
                    IsPrimair = loc.IsPrimair,
                    Naam = loc.Naam,
                    Locatietype = loc.Locatietype,
                    Adres = loc.Adres is null
                        ? null
                        : new Adres
                        {
                            JsonLdMetadata = new JsonLdMetadata
                            {
                                Id = JsonLdType.Adres.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.VCode, loc.LocatieId.ToString()),
                                Type = JsonLdType.Adres.Type,
                            },
                            Straatnaam = loc.Adres.Straatnaam,
                            Huisnummer = loc.Adres.Huisnummer,
                            Busnummer = loc.Adres.Busnummer,
                            Postcode = loc.Adres.Postcode,
                            Gemeente = loc.Adres.Gemeente,
                            Land = loc.Adres.Land,
                        },
                    Adresvoorstelling = loc.Adres.ToAdresString(),
                    AdresId = loc.AdresId is null
                        ? null
                        : new AdresId
                        {
                            Broncode = loc.AdresId?.Broncode,
                            Bronwaarde = loc.AdresId?.Bronwaarde,
                        },
                    VerwijstNaar = loc.AdresId is null
                        ? null
                        : new AdresVerwijzing
                        {
                            JsonLdMetadata = new JsonLdMetadata
                            {
                                Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(loc.AdresId?.Bronwaarde.Split('/').Last()),
                                Type = JsonLdType.AdresVerwijzing.Type,
                            },
                        },
                    Bron = Bron.Initiator,
                }).ToArray(),
            Vertegenwoordigers = feitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.Select(
                v => new Vertegenwoordiger
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.VCode, v.VertegenwoordigerId.ToString()),
                        Type = JsonLdType.Vertegenwoordiger.Type,
                    },
                    VertegenwoordigerId = v.VertegenwoordigerId,
                    Insz = v.Insz,
                    IsPrimair = v.IsPrimair,
                    Roepnaam = v.Roepnaam,
                    Rol = v.Rol,
                    Achternaam = v.Achternaam,
                    Voornaam = v.Voornaam,
                    Email = v.Email,
                    Telefoon = v.Telefoon,
                    Mobiel = v.Mobiel,
                    SocialMedia = v.SocialMedia,
                    Bron = Bron.Initiator,
                    VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(
                                feitelijkeVerenigingWerdGeregistreerd.VCode, v.VertegenwoordigerId.ToString()),
                            Type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                        },
                        IsPrimair = v.IsPrimair,
                        Email = v.Email,
                        Telefoon = v.Telefoon,
                        Mobiel = v.Mobiel,
                        SocialMedia = v.SocialMedia,
                    },
                }).ToArray(),
            HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket.Select(
                h => new HoofdactiviteitVerenigingsloket
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.Hoofdactiviteit.CreateWithIdValues(h.Code),
                        Type = JsonLdType.Hoofdactiviteit.Type,
                    },
                    Code = h.Code,
                    Naam = h.Naam,
                }).ToArray(),
            Werkingsgebieden = [],
            Sleutels = new Sleutel[]
            {
                new()
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.Sleutel.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.VCode, Sleutelbron.VR.Waarde),
                        Type = JsonLdType.Sleutel.Type,
                    },
                    Bron = Sleutelbron.VR.Waarde,
                    Waarde = feitelijkeVerenigingWerdGeregistreerd.VCode,
                    CodeerSysteem = CodeerSysteem.VR,
                    GestructureerdeIdentificator = new GestructureerdeIdentificator
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.VCode, Sleutelbron.VR.Waarde),
                            Type = JsonLdType.GestructureerdeSleutel.Type,
                        },
                        Nummer = feitelijkeVerenigingWerdGeregistreerd.VCode,
                    },
                },
            },
            Relaties = Array.Empty<Relatie>(),
            Bron = Bron.Initiator,
            Metadata = new Metadata(1, 1),
        };
    }
}
