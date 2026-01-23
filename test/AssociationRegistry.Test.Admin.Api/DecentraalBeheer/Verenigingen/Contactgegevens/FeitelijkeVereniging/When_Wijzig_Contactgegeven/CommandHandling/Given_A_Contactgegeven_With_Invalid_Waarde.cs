namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Wijzig_Contactgegeven.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.WijzigContactgegeven;
using AssociationRegistry.DecentraalBeheer.Vereniging.Emails.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Websites;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class Given_A_Contactgegeven_With_Invalid_Waarde
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;

    public Given_A_Contactgegeven_With_Invalid_Waarde()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();
        var verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new WijzigContactgegevenCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_it_throws_an_InvalidEmailFormatException()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario.ContactgegevenId,
                _fixture.Create<Website>().Waarde,
                _fixture.Create<string?>(),
                IsPrimair: false
            )
        );

        var method = () =>
            _commandHandler.Handle(
                new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>())
            );

        await method.Should().ThrowAsync<EmailHeeftEenOngeldigFormaat>();
    }
}
