﻿namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Framework;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Formatters;
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
        var fixture = new Fixture().CustomizePublicApi();
        var feitelijkeVerenigingWerdGeregistreerd = new TestEvent<FeitelijkeVerenigingWerdGeregistreerd>(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>());

        var doc = PubliekVerenigingDetailProjector.Create(feitelijkeVerenigingWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new PubliekVerenigingDetailDocument
            {
                VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                Type = new PubliekVerenigingDetailDocument.VerenigingsType
                {
                    Code = Verenigingstype.FeitelijkeVereniging.Code,
                    Beschrijving = Verenigingstype.FeitelijkeVereniging.Beschrijving,
                },
                Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
                KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
                Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum,
                DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
                Status = "Actief",
                IsUitgeschrevenUitPubliekeDatastroom = feitelijkeVerenigingWerdGeregistreerd.Data.IsUitgeschrevenUitPubliekeDatastroom,
                Contactgegevens = feitelijkeVerenigingWerdGeregistreerd.Data.Contactgegevens.Select(
                    c => new PubliekVerenigingDetailDocument.Contactgegeven
                    {
                        ContactgegevenId = c.ContactgegevenId,
                        Type = c.Type.ToString(),
                        Waarde = c.Waarde,
                        Beschrijving = c.Beschrijving,
                        IsPrimair = c.IsPrimair,
                    }).ToArray(),
                Locaties = feitelijkeVerenigingWerdGeregistreerd.Data.Locaties.Select(
                    loc => new PubliekVerenigingDetailDocument.Locatie
                    {
                        LocatieId = loc.LocatieId,
                        IsPrimair = loc.IsPrimair,
                        Naam = loc.Naam,
                        Locatietype = loc.Locatietype,
                        Adres = loc.Adres is null
                            ? null
                            : new PubliekVerenigingDetailDocument.Adres
                            {
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
                            : new PubliekVerenigingDetailDocument.AdresId
                            {
                                Broncode = loc.AdresId?.Broncode,
                                Bronwaarde = loc.AdresId?.Bronwaarde,
                            },
                    }).ToArray(),
                HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(
                    arg => new PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
                    {
                        Code = arg.Code,
                        Beschrijving = arg.Beschrijving,
                    }).ToArray(),
                Sleutels = Array.Empty<PubliekVerenigingDetailDocument.Sleutel>(),
            });
    }
}
