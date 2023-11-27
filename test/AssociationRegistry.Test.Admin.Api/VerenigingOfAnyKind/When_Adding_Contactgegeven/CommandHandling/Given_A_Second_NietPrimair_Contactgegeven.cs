﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Adding_Contactgegeven.CommandHandling;

using Acties.VoegContactgegevenToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_NietPrimair_Contactgegeven
{
    private readonly VoegContactgegevenToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_A_Second_NietPrimair_Contactgegeven()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegContactgegevenToeCommandHandler(_verenigingRepositoryMock);
    }

    [Theory]
    [InlineData("E-mail", "email2@example.org")]
    [InlineData("Website", "https://www.example.org")]
    [InlineData("SocialMedia", "https://www.example.org")]
    [InlineData("Telefoon", "0000112233")]
    public async Task Then_A_ContactgegevenWerdToegevoegd_Event_Is_Saved(string type, string waarde)
    {
        var command = new VoegContactgegevenToeCommand(
            _scenario.VCode,
            Contactgegeven.CreateFromInitiator(
                Contactgegeventype.Parse(type),
                waarde,
                _fixture.Create<string?>(),
                isPrimair: false));

        await _commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenWerdToegevoegd(ContactgegevenId: 2, command.Contactgegeven.Contactgegeventype, command.Contactgegeven.Waarde,
                                             command.Contactgegeven.Beschrijving, IsPrimair: false)
        );
    }
}
