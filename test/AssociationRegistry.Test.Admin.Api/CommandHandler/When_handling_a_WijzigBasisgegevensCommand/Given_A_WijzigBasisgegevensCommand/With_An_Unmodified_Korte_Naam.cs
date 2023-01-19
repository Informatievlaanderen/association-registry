namespace AssociationRegistry.Test.Admin.Api.CommandHandler.When_handling_a_WijzigBasisgegevensCommand.Given_A_WijzigBasisgegevensCommand;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using FluentAssertions;
using Vereniging;
using Vereniging.WijzigBasisgegevens;
using Xunit;

public class With_An_Unmodified_Korte_Naam
{
    private const string VCodeValue = "V0001001";
    private const string Naam = "GRUB";
    private const string KorteNaam = "GRUB";
    private readonly VerenigingRepositoryMock.InvocationSave _invocationSave;

    public With_An_Unmodified_Korte_Naam()
    {
        var vereniging = new Vereniging();
        vereniging.Apply(
            new VerenigingWerdGeregistreerd(
                VCodeValue, Naam, KorteNaam, null, null, null, null, null, new DateOnly(2023, 1, 1)));

        var verenigingRepositoryMock = new VerenigingRepositoryMock(vereniging);

        var commandHandler = new WijzigBasisgegevensCommandHandler();
        var command = new WijzigBasisgegevensCommand(VCodeValue, KorteNaam: KorteNaam);
        var commandMetadata = new Fixture().Create<CommandMetadata>();

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            verenigingRepositoryMock)
            .GetAwaiter().GetResult();

        _invocationSave = verenigingRepositoryMock.InvocationsSave.Single();
    }

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
        _invocationSave.Vereniging.UncommittedEvents.Should().BeEquivalentTo(
            Array.Empty<IEvent>(),
            options => options.WithStrictOrdering() );
    }
}
