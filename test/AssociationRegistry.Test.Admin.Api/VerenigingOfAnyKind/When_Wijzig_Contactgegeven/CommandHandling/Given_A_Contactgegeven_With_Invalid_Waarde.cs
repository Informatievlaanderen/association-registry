﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Wijzig_Contactgegeven.CommandHandling;

using Acties.WijzigContactgegeven;
using AssociationRegistry.Framework;
using Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging.Emails.Exceptions;
using Vereniging.Websites;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Contactgegeven_With_Invalid_Waarde
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;

    public Given_A_Contactgegeven_With_Invalid_Waarde()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new WijzigContactgegevenCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_it_throws_an_InvalidEmailFormatException()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.ContactgegevenId,
                _fixture.Create<Website>().Waarde,
                _fixture.Create<string?>(),
                IsPrimair: false));

        var method = ()=> _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>()));

        await method.Should().ThrowAsync<InvalidEmailFormat>();
    }
}
