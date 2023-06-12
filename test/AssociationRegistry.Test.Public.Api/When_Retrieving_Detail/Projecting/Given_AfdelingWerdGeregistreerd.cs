namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Framework;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
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
        var afdelingWerdGeregistreerd = new TestEvent<AfdelingWerdGeregistreerd>(fixture.Create<AfdelingWerdGeregistreerd>());

        var doc = PubliekVerenigingDetailProjector.Create(afdelingWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new PubliekVerenigingDetailDocument
            {
                VCode = afdelingWerdGeregistreerd.Data.VCode,
                Type = new PubliekVerenigingDetailDocument.VerenigingsType
                {
                    Code = Verenigingstype.Afdeling.Code,
                    Beschrijving = Verenigingstype.Afdeling.Beschrijving,
                },
                Naam = afdelingWerdGeregistreerd.Data.Naam,
                KorteNaam = afdelingWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = afdelingWerdGeregistreerd.Data.KorteBeschrijving,
                Startdatum = afdelingWerdGeregistreerd.Data.Startdatum,
                DatumLaatsteAanpassing = afdelingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
                Status = "Actief",
                IsUitgeschrevenUitPubliekeDatastroom = false,
                Contactgegevens = afdelingWerdGeregistreerd.Data.Contactgegevens.Select(
                    c => new PubliekVerenigingDetailDocument.Contactgegeven
                    {
                        ContactgegevenId = c.ContactgegevenId,
                        Type = c.Type.ToString(),
                        Waarde = c.Waarde,
                        Beschrijving = c.Beschrijving,
                        IsPrimair = c.IsPrimair,
                    }).ToArray(),
                Locaties = afdelingWerdGeregistreerd.Data.Locaties.Select(
                    loc => new PubliekVerenigingDetailDocument.Locatie
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
                HoofdactiviteitenVerenigingsloket = afdelingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(
                    arg => new PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
                    {
                        Code = arg.Code,
                        Beschrijving = arg.Beschrijving,
                    }).ToArray(),
                Sleutels = Array.Empty<PubliekVerenigingDetailDocument.Sleutel>(),
                Relaties = new[]
                {
                    new PubliekVerenigingDetailDocument.Relatie
                    {
                        Type = RelatieType.IsAfdelingVan.Beschrijving, AndereVereniging =
                            new PubliekVerenigingDetailDocument.Relatie.GerelateerdeVereniging
                            {
                                KboNummer = afdelingWerdGeregistreerd.Data.Moedervereniging.KboNummer,
                                VCode = afdelingWerdGeregistreerd.Data.Moedervereniging.VCode,
                                Naam = afdelingWerdGeregistreerd.Data.Moedervereniging.Naam,
                            },
                    },
                },
            });
    }
}
