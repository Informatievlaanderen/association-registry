namespace AssociationRegistry.Test.Magda.MagdaClient.When_GeefOnderneming;

using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Constants;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.AutoFixture;
using Common.Configuration;
using Common.Framework;
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
    public async Task Then_It_Returns_GeefOndernemingResponseBody(MagdaOptionsSection magdaOptionsSection)
    {
        var magdaCallReferenceService = new Mock<IMagdaCallReferenceService>();
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;

        magdaCallReferenceService.Setup(x => x.CreateReference(commandMetadata.Initiator, commandMetadata.CorrelationId, KboNummer,
                                                               ReferenceContext.GeefOndernemingDienst0200(
                                                                   aanroependeFunctie),
                                                               It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(_fixture.Create<MagdaCallReference>());

        var facade = new MagdaClient(magdaOptionsSection, magdaCallReferenceService.Object, new NullLogger<MagdaClient>());

        var response = await facade.GeefOnderneming(KboNummer, aanroependeFunctie, commandMetadata, CancellationToken.None);

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
