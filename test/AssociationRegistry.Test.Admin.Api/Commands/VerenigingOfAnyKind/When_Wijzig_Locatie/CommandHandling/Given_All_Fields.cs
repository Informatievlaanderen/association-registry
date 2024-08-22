namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Locatie.CommandHandling;

using Acties.WijzigLocatie;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Framework;
using Grar;
using Marten;
using Moq;
using Vereniging;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_All_Fields
{
    private readonly WijzigLocatieCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly WijzigLocatieCommand.Locatie _locatie;

    public Given_All_Fields()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new WijzigLocatieCommandHandler(_verenigingRepositoryMock,
                                                          Mock.Of<IMartenOutbox>(),
                                                          Mock.Of<IDocumentSession>(),
                                                          Mock.Of<IGrarClient>()
        );

        _locatie = new WijzigLocatieCommand.Locatie(
            _scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
            Locatietype.Correspondentie,
            !_scenario.LocatieWerdToegevoegd.Locatie.IsPrimair,
            _fixture.Create<string>(),
            _fixture.Create<Adres>(),
            AdresId: null);
    }

    [Fact]
    public async Task Then_A_LocatieWerdToegevoegd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = new WijzigLocatieCommand(
            _scenario.VCode,
            _locatie);

        await _commandHandler.Handle(new CommandEnvelope<WijzigLocatieCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new LocatieWerdGewijzigd(
                new Registratiedata.Locatie(
                    _scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
                    _locatie.Locatietype!,
                    _locatie.IsPrimair!.Value,
                    _locatie.Naam!,
                    Registratiedata.Adres.With(_locatie.Adres),
                    Registratiedata.AdresId.With(_locatie.AdresId))
            ));
    }
}
