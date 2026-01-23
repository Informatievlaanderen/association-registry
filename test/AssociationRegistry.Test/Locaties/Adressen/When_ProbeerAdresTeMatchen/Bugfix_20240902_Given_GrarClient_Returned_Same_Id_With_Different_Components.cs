namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Integrations.Grar.AdresMatch;
using AssociationRegistry.Integrations.Grar.Clients;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using AutoFixture.Kernel;
using CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging;
using Xunit;

public class Bugfix_20240902_Given_GrarClient_Returned_Same_Id_With_Different_Components
{
    private static Mock<IGrarClient> SetupGrarClientMock(Fixture fixture, Registratiedata.Locatie locatie)
    {
        var grarClient = new Mock<IGrarClient>();

        grarClient
            .Setup(x =>
                x.GetAddressMatches(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    CancellationToken.None
                )
            )
            .ReturnsAsync(
                new AdresMatchResponseCollection(
                    new[] { fixture.Create<AddressMatchResponse>() with { Score = 100, AdresId = locatie.AdresId } }
                )
            );

        return grarClient;
    }

    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task Then_AdresWerdOvergenomenUitAdressenregister_IsApplied_And_LocatieDuplicaatWerdVerwijderdNaAdresMatch_IsNeverApplied(
        Type verenigingType
    )
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);

        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)
            context.Resolve(verenigingType);

        var locatie = verenigingWerdGeregistreerd.Locaties.First();

        var grarClient = SetupGrarClientMock(fixture, locatie);

        var vereniging = new VerenigingOfAnyKind();

        var state = new VerenigingState().Apply((dynamic)verenigingWerdGeregistreerd);

        vereniging.Hydrate(state);

        var faktory = Faktory.New();

        AggregateSessionMock repository = faktory.AggregateSession.Mock(state, expectedLoadingDubbel: true);

        var handler = new ProbeerAdresTeMatchenCommandHandler(
            repository,
            new AdresMatchService(
                grarClient.Object,
                new PerfectScoreMatchStrategy(),
                new GrarAddressVerrijkingsService(grarClient.Object)
            ),
            NullLogger<ProbeerAdresTeMatchenCommandHandler>.Instance
        );

        await handler.Handle(
            new ProbeerAdresTeMatchenCommand(
                verenigingWerdGeregistreerd.VCode,
                verenigingWerdGeregistreerd.Locaties.ToArray()[0].LocatieId
            )
        );

        repository.ShouldHaveSavedEventType<AdresWerdOvergenomenUitAdressenregister>(1);
        repository.ShouldNotHaveSaved<LocatieDuplicaatWerdVerwijderdNaAdresMatch>();
    }
}
