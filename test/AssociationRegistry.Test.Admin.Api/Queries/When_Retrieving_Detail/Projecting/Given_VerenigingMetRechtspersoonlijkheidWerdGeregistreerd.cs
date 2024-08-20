<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Projector/Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Projecting/Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.cs

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
using Contactgegeven = AssociationRegistry.Admin.Schema.Detail.Contactgegeven;
using Doelgroep = AssociationRegistry.Admin.Schema.Detail.Doelgroep;
using HoofdactiviteitVerenigingsloket = AssociationRegistry.Admin.Schema.Detail.HoofdactiviteitVerenigingsloket;
using Locatie = AssociationRegistry.Admin.Schema.Detail.Locatie;
using Vertegenwoordiger = AssociationRegistry.Admin.Schema.Detail.Vertegenwoordiger;

[UnitTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    [Theory]
    [InlineData("VZW")]
    [InlineData("IVZW")]
    [InlineData("PS")]
    [InlineData("SVON")]
    public void Then_it_creates_a_new_vereniging(string rechtsvorm)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>();

        verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data with
        {
            Rechtsvorm = rechtsvorm,
        };

        var doc = BeheerVerenigingDetailProjector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new BeheerVerenigingDetailDocument
            {
                JsonLdMetadataType = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
                VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                Verenigingstype = new VerenigingsType
                {
                    Code = Verenigingstype.Parse(rechtsvorm).Code,
                    Naam = Verenigingstype.Parse(rechtsvorm).Naam,
                },
                Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
                Roepnaam = string.Empty,
                KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = string.Empty,
                Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
                Doelgroep = new Doelgroep
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.Doelgroep.CreateWithIdValues(doc.VCode),
                        Type = JsonLdType.Doelgroep.Type,
                    },
                    Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                    Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
                },
                Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
                DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Tijdstip.ToBelgianDate(),
                Status = VerenigingStatus.Actief,
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
                            Id = JsonLdType.Sleutel.CreateWithIdValues(doc.VCode, Sleutelbron.VR.Waarde),
                            Type = JsonLdType.Sleutel.Type,
                        },
                        Bron = Sleutelbron.VR.Waarde,
                        Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                        CodeerSysteem = CodeerSysteem.VR.Waarde,
                        GestructureerdeIdentificator = new GestructureerdeIdentificator
                        {
                            JsonLdMetadata = new JsonLdMetadata
                            {
                                Id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(doc.VCode, Sleutelbron.VR.Waarde),
                                Type = JsonLdType.GestructureerdeSleutel.Type,
                            },
                            Nummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                        },
                    },
                    new()
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.Sleutel.CreateWithIdValues(doc.VCode, Sleutelbron.KBO.Waarde),
                            Type = JsonLdType.Sleutel.Type,
                        },
                        Bron = Sleutelbron.KBO.Waarde,
                        Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                        CodeerSysteem = CodeerSysteem.KBO.Waarde,
                        GestructureerdeIdentificator = new GestructureerdeIdentificator
                        {
                            JsonLdMetadata = new JsonLdMetadata
                            {
                                Id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(doc.VCode, Sleutelbron.KBO.Waarde),
                                Type = JsonLdType.GestructureerdeSleutel.Type,
                            },
                            Nummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                        },
                    },
                },
                Relaties = Array.Empty<Relatie>(),
                Bron = Bron.KBO,
                Metadata = new Metadata(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence,
                                        verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version),
            });
    }
}
