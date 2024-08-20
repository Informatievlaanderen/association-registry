﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Contactgegeven.CommandHandling;

using AssociationRegistry.Acties.WijzigContactgegeven;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Emails;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Contactgegeven
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_A_Contactgegeven()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new WijzigContactgegevenCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_ContactgegevenWerdToegevoegd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.ContactgegevenId,
                _fixture.Create<Email>().Waarde,
                _fixture.Create<string?>(),
                IsPrimair: false));

        await _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenWerdGewijzigd(
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.ContactgegevenId,
                Contactgegeventype.Email,
                command.Contactgegeven.Waarde!,
                command.Contactgegeven.Beschrijving ?? string.Empty,
                IsPrimair: false)
        );
    }
}
