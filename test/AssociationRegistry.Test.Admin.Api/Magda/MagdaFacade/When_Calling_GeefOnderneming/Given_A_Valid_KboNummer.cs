﻿namespace AssociationRegistry.Test.Admin.Api.Magda.MagdaFacade.When_Calling_GeefOnderneming;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Configuration;
using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using Framework.Helpers;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Categories;

[IntegrationTest]
public class Given_A_Valid_KboNummer
{
    private const string KboNummer = "0442528054";
    private readonly Fixture _fixture = new();

    [Theory]
    [MemberData(nameof(GetData))]
    public async Task Then_It_Returns_GeefOndernemingResponseBody(MagdaOptionsSection magdaOptionsSection)
    {
        var facade = new MagdaFacade(magdaOptionsSection, new NullLogger<MagdaFacade>());

        var response = await facade.GeefOnderneming(KboNummer, _fixture.Create<MagdaCallReference>());

        using (new AssertionScope())
        {
            var repliek = response?.Body?.GeefOndernemingResponse?.Repliek;
            repliek?.Uitzonderingen.Should().BeNullOrEmpty();

            var onderneming = repliek?.Antwoorden.Antwoord.Inhoud.Onderneming;

            onderneming.Should().NotBeNull();

            onderneming?.Rechtsvormen.Should().ContainSingle(r => r.Code.Value == RechtsvormCodes.VZW);
            onderneming?.Namen.MaatschappelijkeNamen.Should().ContainEquivalentOf(
                new NaamOndernemingType
                {
                    Naam = "Vlaamse Liga tegen Kanker",
                    Taalcode = "nl",
                    DatumBegin = "1998-01-01",
                    DatumEinde = null,
                });
            onderneming?.OndernemingOfVestiging.Code.Value.Should().Be(OndernemingOfVestigingCodes.Onderneming);
            onderneming?.StatusKBO.Code.Value.Should().Be(StatusKBOCodes.Actief);
            onderneming?.SoortOnderneming.Code.Value.Should().Be(SoortOndernemingCodes.Rechtspersoon);
        }
    }

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[]
        {
            ConfigurationHelper.GetConfiguration().GetMagdaOptionsSection("WiremockMagdaOptions"),
        };
        yield return new object[]
        {
            ConfigurationHelper.GetConfiguration().GetMagdaOptionsSection("LiveMagdaOptions"),
        };
    }
}
