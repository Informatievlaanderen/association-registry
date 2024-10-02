namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Bronnen;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Schema.Detail.Adres;
using AdresId = AssociationRegistry.Admin.Schema.Detail.AdresId;
using Contactgegeven = AssociationRegistry.Admin.Schema.Detail.Contactgegeven;
using Doelgroep = AssociationRegistry.Admin.Schema.Detail.Doelgroep;
using HoofdactiviteitVerenigingsloket = AssociationRegistry.Admin.Schema.Detail.HoofdactiviteitVerenigingsloket;
using Locatie = AssociationRegistry.Admin.Schema.Detail.Locatie;
using Vertegenwoordiger = AssociationRegistry.Admin.Schema.Detail.Vertegenwoordiger;

[UnitTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_vereniging()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<TestEvent<FeitelijkeVerenigingWerdGeregistreerd>>();

        var doc = BeheerVerenigingDetailProjector.Create(feitelijkeVerenigingWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new BeheerVerenigingDetailDocument
            {
                JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type,
                VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                Verenigingstype = new VerenigingsType
                {
                    Code = Verenigingstype.FeitelijkeVereniging.Code,
                    Naam = Verenigingstype.FeitelijkeVereniging.Naam,
                },
                Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
                KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
                Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
                Doelgroep = new Doelgroep
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.Doelgroep.CreateWithIdValues(doc.VCode),
                        Type = JsonLdType.Doelgroep.Type,
                    },
                    Minimumleeftijd = feitelijkeVerenigingWerdGeregistreerd.Data.Doelgroep.Minimumleeftijd,
                    Maximumleeftijd = feitelijkeVerenigingWerdGeregistreerd.Data.Doelgroep.Maximumleeftijd,
                },
                DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.Tijdstip.FormatAsBelgianDate(),
                Status = VerenigingStatus.Actief,
                Contactgegevens = feitelijkeVerenigingWerdGeregistreerd.Data.Contactgegevens.Select(
                    c => new Contactgegeven
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.Contactgegeven.CreateWithIdValues(doc.VCode, c.ContactgegevenId.ToString()),
                            Type = JsonLdType.Contactgegeven.Type,
                        },
                        ContactgegevenId = c.ContactgegevenId,
                        Contactgegeventype = c.Contactgegeventype.ToString(),
                        Waarde = c.Waarde,
                        Beschrijving = c.Beschrijving,
                        IsPrimair = c.IsPrimair,
                        Bron = Bron.Initiator,
                    }).ToArray(),
                Locaties = feitelijkeVerenigingWerdGeregistreerd.Data.Locaties.Select(
                    loc => new Locatie
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.Locatie.CreateWithIdValues(doc.VCode, loc.LocatieId.ToString()),
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
                                    Id = JsonLdType.Adres.CreateWithIdValues(doc.VCode, loc.LocatieId.ToString()),
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
                Vertegenwoordigers = feitelijkeVerenigingWerdGeregistreerd.Data.Vertegenwoordigers.Select(
                    v => new Vertegenwoordiger
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(doc.VCode, v.VertegenwoordigerId.ToString()),
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
                                    doc.VCode, v.VertegenwoordigerId.ToString()),
                                Type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                            },
                            IsPrimair = v.IsPrimair,
                            Email = v.Email,
                            Telefoon = v.Telefoon,
                            Mobiel = v.Mobiel,
                            SocialMedia = v.SocialMedia,
                        },
                    }).ToArray(),
                HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(
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
                Sleutels = new Sleutel[]
                {
                    new()
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.Sleutel.CreateWithIdValues(doc.VCode, Sleutelbron.VR.Waarde),
                            Type = JsonLdType.Sleutel.Type,
                        },
                        Bron = Sleutelbron.VR.Waarde,
                        Waarde = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                        CodeerSysteem = CodeerSysteem.VR,
                        GestructureerdeIdentificator = new GestructureerdeIdentificator
                        {
                            JsonLdMetadata = new JsonLdMetadata
                            {
                                Id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(doc.VCode, Sleutelbron.VR.Waarde),
                                Type = JsonLdType.GestructureerdeSleutel.Type,
                            },
                            Nummer = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                        },
                    },
                },
                Relaties = Array.Empty<Relatie>(),
                Bron = Bron.Initiator,
                Metadata = new Metadata(feitelijkeVerenigingWerdGeregistreerd.Sequence, feitelijkeVerenigingWerdGeregistreerd.Version),
            });
    }
}
