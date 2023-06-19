namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_AfdelingWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_vereniging()
    {
        var fixture = new Fixture().CustomizeAll();
        var afdelingWerdGeregistreerd = fixture.Create<TestEvent<AfdelingWerdGeregistreerd>>();

        var doc = BeheerVerenigingDetailProjector.Create(afdelingWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new BeheerVerenigingDetailDocument
            {
                VCode = afdelingWerdGeregistreerd.Data.VCode,
                Type = new BeheerVerenigingDetailDocument.VerenigingsType
                {
                    Code = Verenigingstype.Afdeling.Code,
                    Beschrijving = Verenigingstype.Afdeling.Beschrijving,
                },
                Naam = afdelingWerdGeregistreerd.Data.Naam,
                KorteNaam = afdelingWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = afdelingWerdGeregistreerd.Data.KorteBeschrijving,
                Startdatum = afdelingWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
                DatumLaatsteAanpassing = afdelingWerdGeregistreerd.Tijdstip.ToBelgianDate(),
                Status = "Actief",
                Contactgegevens = afdelingWerdGeregistreerd.Data.Contactgegevens.Select(
                    c => new BeheerVerenigingDetailDocument.Contactgegeven
                    {
                        ContactgegevenId = c.ContactgegevenId,
                        Type = c.Type.ToString(),
                        Waarde = c.Waarde,
                        Beschrijving = c.Beschrijving,
                        IsPrimair = c.IsPrimair,
                    }).ToArray(),
                Locaties = afdelingWerdGeregistreerd.Data.Locaties.Select(
                    loc => new BeheerVerenigingDetailDocument.Locatie
                    {
                        LocatieId = loc.LocatieId,
                        Hoofdlocatie = loc.Hoofdlocatie,
                        Naam = loc.Naam,
                        Locatietype = loc.Locatietype,
                        Straatnaam = loc.Straatnaam,
                        Huisnummer = loc.Huisnummer,
                        Busnummer = loc.Busnummer,
                        Postcode = loc.Postcode,
                        Gemeente = loc.Gemeente,
                        Land = loc.Land,
                        Adres = loc.ToAdresString(),
                    }).ToArray(),
                Vertegenwoordigers = afdelingWerdGeregistreerd.Data.Vertegenwoordigers.Select(
                    v => new BeheerVerenigingDetailDocument.Vertegenwoordiger
                    {
                        VertegenwoordigerId = v.VertegenwoordigerId,
                        IsPrimair = v.IsPrimair,
                        Roepnaam = v.Roepnaam,
                        Rol = v.Rol,
                        Achternaam = v.Achternaam,
                        Voornaam = v.Voornaam,
                        Email = v.Email,
                        Telefoon = v.Telefoon,
                        Mobiel = v.Mobiel,
                        SocialMedia = v.SocialMedia,
                    }).ToArray(),
                HoofdactiviteitenVerenigingsloket = afdelingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(
                    h => new BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
                    {
                        Code = h.Code,
                        Beschrijving = h.Beschrijving,
                    }).ToArray(),
                Sleutels = Array.Empty<BeheerVerenigingDetailDocument.Sleutel>(),
                Relaties = new[]
                {
                    new BeheerVerenigingDetailDocument.Relatie
                    {
                        Type = RelatieType.IsAfdelingVan.Beschrijving,
                        AndereVereniging = new BeheerVerenigingDetailDocument.Relatie.GerelateerdeVereniging
                        {
                            KboNummer = afdelingWerdGeregistreerd.Data.Moedervereniging.KboNummer,
                            VCode = afdelingWerdGeregistreerd.Data.Moedervereniging.VCode,
                            Naam = afdelingWerdGeregistreerd.Data.Moedervereniging.Naam,
                        },
                    },
                },
                Metadata = new Metadata(afdelingWerdGeregistreerd.Sequence, afdelingWerdGeregistreerd.Version),
            });
        doc.DatumLaatsteAanpassing.Should().Be(afdelingWerdGeregistreerd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(afdelingWerdGeregistreerd.Sequence, afdelingWerdGeregistreerd.Version));
    }
}
