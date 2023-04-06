namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using ContactGegevens;
using ContactGegevens.Exceptions;
using Fakes;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using Vereniging.VoegContactgegevenToe;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Contactgegeven_With_Same_Type_And_Value_But_Diff_Omschrijving
{
    private readonly VoegContactgegevenToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGeregistreerd_Commandhandler_Scenario _scenario;

    public Given_A_Contactgegeven_With_Same_Type_And_Value_But_Diff_Omschrijving()
    {
        _scenario = new VerenigingWerdGeregistreerd_Commandhandler_Scenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new VoegContactgegevenToeCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_DuplicateContactgegeven_Is_Not_Thrown()
    {
        var command = new VoegContactgegevenToeCommand(
            _scenario.VCode,
            new VoegContactgegevenToeCommand.CommandContactgegeven(
                ContactgegevenType.Website,
                "https://example.org",
                _fixture.Create<string?>(),
                false));

        await _commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>()));

        var secondVoegContactGegevenToeCall = async () =>
        {
            var sameCommandButWithDifferentOmschrijving = command with
            {
                Contactgegeven = command.Contactgegeven with
                {
                    Omschrijving = _fixture.Create<string>()
                }
            };
            return await _commandHandler.Handle(
                new CommandEnvelope<VoegContactgegevenToeCommand>(
                    sameCommandButWithDifferentOmschrijving,
                    _fixture.Create<CommandMetadata>()));
        };

        await secondVoegContactGegevenToeCall.Should()
            .NotThrowAsync<DuplicateContactgegeven>();
    }
}
