﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Contactgegeven.CommandHandling;

using Acties.VoegContactgegevenToe;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_NietPrimair_Contactgegeven
{
    [Theory]
    [InlineData("E-mail", "email@example.org")]
    [InlineData("Website", "https://www.example.org")]
    [InlineData("SocialMedia", "https://www.example.org")]
    [InlineData("Telefoon", "0000112233")]
    public async Task Then_A_ContactgegevenWerdToegevoegd_Event_Is_Saved(string type, string waarde)
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var commandHandler = new VoegContactgegevenToeCommandHandler(verenigingRepositoryMock);
        var command = new VoegContactgegevenToeCommand(
            scenario.VCode,
            Contactgegeven.Create(
                type,
                waarde,
                fixture.Create<string>(),
                false));

        await commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenWerdToegevoegd(
                scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens.Max(c => c.ContactgegevenId) + 1,
                command.Contactgegeven.Type,
                command.Contactgegeven.Waarde,
                command.Contactgegeven.Beschrijving,
                false)
        );
    }
}
