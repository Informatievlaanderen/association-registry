namespace AssociationRegistry.Test.Magda.MagdaClient.When_GeefOnderneming;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.Configuration;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using Integrations.Magda.Onderneming.GeefOnderneming;
using Integrations.Magda.Shared.Constants;
using Xunit;

public class Given_A_Valid_KboNummer
{
    private const string KboNummer = "0442528054";
    private readonly Fixture _fixture = new();

    [IgnoreMagdaTestsTheory]
    [MemberData(nameof(GetData))]
    public async Task Then_It_Returns_GeefOndernemingResponseBody(MagdaOptionsSection magdaOptionsSection)
    {
        var commandMetadata = _fixture.Create<CommandMetadata>();
        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;
        var magdaClient = MagdaClientTestSetup.CreateMagdaClient(_fixture, commandMetadata, KboNummer);

        var response = await magdaClient.GeefOnderneming(KboNummer, aanroependeFunctie, commandMetadata, CancellationToken.None);

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

            onderneming?.Bankrekeningen.Should().BeEquivalentTo([
                new BankrekeningType()
                {
                    Rekeningnummer = "000 1111117 96",
                    Doel = "003",
                    IBAN = "BE68539007547034",
                    BIC = "ABCABE99",
                    DatumBegin = "1930-12-01",
                },
                new BankrekeningType()
                {
                    Rekeningnummer = "999 1111117 96",
                    Doel = "003",
                    IBAN = "BE68539007547555",
                    BIC = "ABCABE11",
                    DatumBegin = "1930-12-02",
                    DatumEinde = "2000-02-01",
                },
            ]);
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
