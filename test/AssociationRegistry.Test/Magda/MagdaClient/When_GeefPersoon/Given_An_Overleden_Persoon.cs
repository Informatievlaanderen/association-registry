namespace AssociationRegistry.Test.Magda.MagdaClient.When_GeefPersoon;

using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Test.Common.Configuration;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using ResultaatEnumType = Integrations.Magda.Repertorium.RegistreerInschrijving0200.ResultaatEnumType;

public class Given_An_Overleden_Persoon
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task Then_It_Returns_ResponsePersoonsBody()
    {
        var insz = "01234567890";
        var commandMetadata = _fixture.Create<CommandMetadata>();
        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;
        var magdaClient = MagdaClientTestSetup.CreateMagdaClient(_fixture, commandMetadata, insz);

        var response = await magdaClient.GeefPersoon(insz, aanroependeFunctie, commandMetadata, CancellationToken.None);

        using (new AssertionScope())
        {
            var repliek = response?.Body?.GeefPersoonResponse?.Repliek;
            repliek?.Uitzonderingen.Should().BeNullOrEmpty();

            var resultaat = repliek?.Antwoorden.Antwoord.Inhoud.Persoon;

            resultaat.Should().NotBeNull();
            resultaat.INSZ.Should().Be(insz);
            resultaat.Naam.Achternamen.First().Value.Should().Be("Naam");
            resultaat.Naam.Voornamen.First().Value.Should().Be("Voornaam");
            resultaat.Overlijden.Datum.Should().Be("1986-01-01");
        }
    }
}
