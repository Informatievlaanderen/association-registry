namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Wijzig_Locatie.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Locaties.WijzigLocatie;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Marten;
using Moq;
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
    public async ValueTask Then_A_LocatieWerdToegevoegd_Event_Is_Saved_With_The_Next_Id()
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
                    EventFactory.Adres(_locatie.Adres),
                    EventFactory.AdresId(_locatie.AdresId))
            ));
    }
}
