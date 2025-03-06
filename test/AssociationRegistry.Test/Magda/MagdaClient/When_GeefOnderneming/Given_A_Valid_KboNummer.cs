namespace AssociationRegistry.Test.Magda.MagdaClient.When_GeefOnderneming;

using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Models;
using AutoFixture;
using Common.Configuration;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

[IntegrationTest]
public class Given_A_Valid_KboNummer
{
    private const string KboNummer = "0442528054";
    private readonly Fixture _fixture = new();

    [IgnoreMagdaTestsTheory]
    [MemberData(nameof(GetData))]
    public async Task Then_It_Returns_GeefOndernemingResponseBody(MagdaOptionsSection magdaOptionsSection)
    {
        var facade = new MagdaClient(magdaOptionsSection, new NullLogger<MagdaClient>());

        var response = await facade.GeefOnderneming(KboNummer, _fixture.Create<MagdaCallReference>());

        using (new AssertionScope())
        {
            var repliek = response?.Body?.GeefOndernemingResponse?.Repliek;
            repliek?.Uitzonderingen.Should().BeNullOrEmpty();

            var onderneming = repliek?.Antwoorden.Antwoord.Inhoud.Onderneming;

            onderneming.Should().NotBeNull();

            onderneming?.Rechtsvormen.Should().ContainSingle(r => r.Code.Value == RechtsvormCodes.VZW);

            onderneming?.Namen.MaatschappelijkeNamen
                        .Count(mn => !string.IsNullOrEmpty(mn.Naam))
                        .Should().BeGreaterThan(0);

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
