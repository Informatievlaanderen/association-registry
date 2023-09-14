namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Adding_Contactgegeven.CommandHandling;

using Acties.VoegContactgegevenToe;
using AssociationRegistry.Framework;
using Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_Primair_Contactgegeven_Of_The_Same_Type
{
    private readonly VoegContactgegevenToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;

    public Given_A_Second_Primair_Contactgegeven_Of_The_Same_Type()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegContactgegevenToeCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_MultiplePrimaryContactgegevens_Is_Thrown()
    {
        var command = new VoegContactgegevenToeCommand(
            _scenario.VCode,
            Contactgegeven.Create(
                ContactgegevenType.Labels.Email,
                "test2@example.org",
                _fixture.Create<string?>(),
                true));

        var handleCall = async () => await _commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>()));

        await handleCall.Should()
            .ThrowAsync<MeerderePrimaireContactgegevensZijnNietToegestaan>()
            .WithMessage(new MeerderePrimaireContactgegevensZijnNietToegestaan(ContactgegevenType.Email.ToString()).Message);
    }
}
