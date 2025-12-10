namespace AssociationRegistry.Test.Magda.MagdaClient;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.Configuration;
using Hosts.Configuration;
using Integrations.Magda;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Persoon.Validation;
using Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public static class MagdaClientTestSetup
{
    public static MagdaClient CreateMagdaClient(Fixture fixture, CommandMetadata commandMetadata, string subject)
    {
        var magdaOptionsSection = ConfigurationHelper.GetConfiguration().GetMagdaOptionsSection("WiremockMagdaOptions");

        var magdaCallReferenceService = new Mock<IMagdaCallReferenceService>();

        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;

        magdaCallReferenceService.Setup(x => x.CreateReference(commandMetadata.Initiator, commandMetadata.CorrelationId, subject,
                                                               ReferenceContext.GeefPersoon0200(
                                                                   aanroependeFunctie),
                                                               It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(fixture.Create<MagdaCallReference>());

        magdaCallReferenceService.Setup(x => x.CreateReference(commandMetadata.Initiator, commandMetadata.CorrelationId, subject,
                                                               ReferenceContext.RegistreerInschrijving0200(
                                                                   aanroependeFunctie),
                                                               It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(fixture.Create<MagdaCallReference>());

        magdaCallReferenceService.Setup(x => x.CreateReference(commandMetadata.Initiator, commandMetadata.CorrelationId, subject,
                                                               ReferenceContext.RegistreerInschrijving0201(
                                                                   aanroependeFunctie),
                                                               It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(fixture.Create<MagdaCallReference>());

        magdaCallReferenceService.Setup(x => x.CreateReference(commandMetadata.Initiator, commandMetadata.CorrelationId, subject,
                                                               ReferenceContext.GeefOndernemingDienst0200(
                                                                   aanroependeFunctie),
                                                               It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(fixture.Create<MagdaCallReference>());


        return new MagdaClient(magdaOptionsSection,
                                          magdaCallReferenceService.Object,
                                          new MagdaRegistreerInschrijvingValidator(NullLogger<MagdaRegistreerInschrijvingValidator>.Instance),
                                          new MagdaGeefPersoonValidator(NullLogger<MagdaGeefPersoonValidator>.Instance),
                                          new NullLogger<MagdaClient>());
    }
}
