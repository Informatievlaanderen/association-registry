namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Contactgegeven.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using DecentraalBeheer.Contactgegevens.VoegContactgegevenToe;
using Events;
using FluentAssertions;
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

    [Fact]
    public async Task Then_A_ContactgegevenWerdToegevoegd_Event_Is_Saved()
    {
        var command = new VoegContactgegevenToeCommand(
            _scenario.VCode,
            Contactgegeven.CreateFromInitiator(
                Contactgegeventype.Parse("SocialMedia"),
                "https://www.example.org",
                _fixture.Create<string?>(),
                isPrimair: false));

        var result = await _commandHandler.Handle(
            new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>()));

        var contactgegevenId = _verenigingRepositoryMock.SaveInvocations[0].Vereniging.UncommittedEvents.ToArray()[0]
                                                        .As<ContactgegevenWerdToegevoegd>()
                                                        .ContactgegevenId;

        result.EntityId.Should().Be(contactgegevenId);
    }
}
