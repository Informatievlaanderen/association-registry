namespace AssociationRegistry.Test.Admin.Api.Magda.When_RegistreerInschrijving.Client;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Repertorium.RegistreerInschrijving;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Helpers;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Categories;

[IntegrationTest]
public class Given_A_Valid_KboNummer
{
    private const string KboNummer = "0442528054";
    private readonly Fixture _fixture = new();

    [IgnoreMagdaTestsTheory]
    [MemberData(nameof(GetData))]
    public async Task Then_It_Returns_RegistreerInschrijvingResponseBody(MagdaOptionsSection magdaOptionsSection)
    {
        var facade = new MagdaClient(magdaOptionsSection, new NullLogger<MagdaClient>());

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
