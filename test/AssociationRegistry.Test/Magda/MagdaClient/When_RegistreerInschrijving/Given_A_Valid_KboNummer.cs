namespace AssociationRegistry.Test.Magda.MagdaClient.When_RegistreerInschrijving;

using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Models;
using Integrations.Magda.Repertorium.RegistreerInschrijving0201;
using AutoFixture;
using Common.Configuration;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_A_Valid_KboNummer
{
    private const string KboNummer = "0442528054";
    private readonly Fixture _fixture = new();

    [IgnoreMagdaTestsTheory]
    [MemberData(nameof(GetData))]
    public async Task Then_It_Returns_RegistreerInschrijvingResponseBody(MagdaOptionsSection magdaOptionsSection)
    {
        var facade = new MagdaClient(magdaOptionsSection, Mock.Of<IMagdaCallReferenceRepository>(), new NullLogger<MagdaClient>());

        var response = await facade.RegistreerInschrijving(KboNummer, _fixture.Create<MagdaCallReference>());

        using (new AssertionScope())
        {
            var repliek = response?.Body?.RegistreerInschrijvingResponse?.Repliek;
            repliek?.Uitzonderingen.Should().BeNullOrEmpty();

            var resultaat = repliek?.Antwoorden.Antwoord.Inhoud.Resultaat;

            resultaat.Should().NotBeNull();
            resultaat.Value.Should().BeOneOf(ResultaatEnumType.Item0, ResultaatEnumType.Item1);
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
