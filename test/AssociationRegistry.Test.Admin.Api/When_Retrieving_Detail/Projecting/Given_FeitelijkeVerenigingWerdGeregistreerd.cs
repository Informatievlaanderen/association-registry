namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_vereniging()
    {
        var fixture = new Fixture().CustomizeAll();
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<TestEvent<FeitelijkeVerenigingWerdGeregistreerd>>();
        var projector = new BeheerVerenigingDetailProjection();

        var doc = projector.Create(feitelijkeVerenigingWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new BeheerVerenigingDetailDocument
            {
                VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                Type = new BeheerVerenigingDetailDocument.VerenigingsType
                {
                    Code = VerenigingsType.FeitelijkeVereniging.Code,
                    Beschrijving = VerenigingsType.FeitelijkeVereniging.Beschrijving,
                },
                Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
                KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
                Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
                DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.Tijdstip.ToBelgianDate(),
                Status = "Actief",
                Contactgegevens = feitelijkeVerenigingWerdGeregistreerd.Data.Contactgegevens.Select(
                    c => new BeheerVerenigingDetailDocument.Contactgegeven
                    {
                        ContactgegevenId = c.ContactgegevenId,
                        Type = c.Type.ToString(),
                        Waarde = c.Waarde,
                        Beschrijving = c.Beschrijving,
                        IsPrimair = c.IsPrimair,
                    }).ToArray(),
                Locaties = feitelijkeVerenigingWerdGeregistreerd.Data.Locaties.Select(
                    loc => new BeheerVerenigingDetailDocument.Locatie
                    {
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
                Vertegenwoordigers = feitelijkeVerenigingWerdGeregistreerd.Data.Vertegenwoordigers.Select(
                    v => new BeheerVerenigingDetailDocument.Vertegenwoordiger
                    {
                        VertegenwoordigerId = v.VertegenwoordigerId,
                        IsPrimair = v.IsPrimair,
                        Roepnaam = v.Roepnaam,
                        Insz = v.Insz,
                        Rol = v.Rol,
                        Achternaam = v.Achternaam,
                        Voornaam = v.Voornaam,
                        Email = v.Email,
                        Telefoon = v.Telefoon,
                        Mobiel = v.Mobiel,
                        SocialMedia = v.SocialMedia,
                    }).ToArray(),
                HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(
                    h => new BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
                    {
                        Code = h.Code,
                        Beschrijving = h.Beschrijving,
                    }).ToArray(),
                Sleutels = Array.Empty<BeheerVerenigingDetailDocument.Sleutel>(),
                Metadata = new Metadata(feitelijkeVerenigingWerdGeregistreerd.Sequence, feitelijkeVerenigingWerdGeregistreerd.Version),
            });
        doc.DatumLaatsteAanpassing.Should().Be(feitelijkeVerenigingWerdGeregistreerd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(feitelijkeVerenigingWerdGeregistreerd.Sequence, feitelijkeVerenigingWerdGeregistreerd.Version));}
}
