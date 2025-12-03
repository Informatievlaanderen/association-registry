namespace AssociationRegistry.Test.Magda.MagdaClient.When_RegistreerInschrijvingOnderneming;

using AssociationRegistry.Framework;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Integrations.Magda.Repertorium.RegistreerInschrijving0201;
using AssociationRegistry.Magda.Kbo;
using Common.Configuration;
using Framework;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
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
        var magdaCallReferenceService = new Mock<IMagdaCallReferenceService>();
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;

        magdaCallReferenceService.Setup(x => x.CreateReference(commandMetadata.Initiator, commandMetadata.CorrelationId, KboNummer,
                                                               ReferenceContext.RegistreerInschrijving0201(
                                                                   aanroependeFunctie),
                                                               It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(_fixture.Create<MagdaCallReference>());

        var facade = new MagdaClient(magdaOptionsSection, magdaCallReferenceService.Object, new NullLogger<MagdaClient>());

        var response = await facade.RegistreerInschrijvingOnderneming(KboNummer, aanroependeFunctie, commandMetadata, CancellationToken.None);

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
