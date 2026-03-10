namespace AssociationRegistry.Test.MagdaSync.KszSync;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Persoonsgegevens;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using CommandHandling.MagdaSync.SyncKsz;
using CommandHandling.MagdaSync.SyncKsz.Queries;
using Common.StubsMocksFakes.Faktories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;
using Wolverine.Marten;
using Xunit;

public class Given_Already_Removed_Vertegenwoordiger
{
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly SyncKszMessageHandler _sut;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_Already_Removed_Vertegenwoordiger()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario =
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();
        var teVerwijderenVertegenwoordiger = _scenario.VertegenwoordigerWerdToegevoegd;
        var state = _scenario.GetVerenigingState();

        state = state.Apply(
            new VertegenwoordigerWerdVerwijderd(
                teVerwijderenVertegenwoordiger.VertegenwoordigerId,
                teVerwijderenVertegenwoordiger.Insz,
                teVerwijderenVertegenwoordiger.Voornaam,
                teVerwijderenVertegenwoordiger.Achternaam
            )
        );

        var magdaGeefPersoonService = Faktory.New(_fixture).MagdaGeefPersoonService.ReturnsOverledenPersoon();

        _aggregateSessionMock = new AggregateSessionMock(state, true, true);

        var persoonsgegevensRepoMock = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();
        persoonsgegevensRepoMock
            .Setup(x =>
                x.Get(Insz.Create(_scenario.VertegenwoordigerWerdToegevoegd.Insz), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new[]
                {
                    _fixture.Create<VertegenwoordigerPersoonsgegevens>() with
                    {
                        VCode = _scenario.VCode,
                        Insz = teVerwijderenVertegenwoordiger.Insz,
                        VertegenwoordigerId = _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                    },
                }
            );

        var filterVzerOnylQueryMock = new Mock<IFilterVzerOnlyQuery>();

        filterVzerOnylQueryMock
            .Setup(x => x.ExecuteAsync(It.IsAny<FilterVzerOnlyQueryFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([_scenario.VCode]);

        _sut = new SyncKszMessageHandler(
            new VzerVertegenwoordigerForInszQuery(
                persoonsgegevensRepoMock.Object,
                filterVzerOnylQueryMock.Object,
                NullLogger<VzerVertegenwoordigerForInszQuery>.Instance
            ),
            _aggregateSessionMock,
            magdaGeefPersoonService.Object,
            NullLogger<SyncKszMessageHandler>.Instance
        );

        _sut.Handle(
                new CommandEnvelope<SyncKszMessage>(
                    new SyncKszMessage(Insz.Hydrate(teVerwijderenVertegenwoordiger.Insz), Guid.NewGuid()),
                    TestCommandMetadata.ForDigitaalVlaanderenProcess
                ),
                CancellationToken.None
            )
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
        _aggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
