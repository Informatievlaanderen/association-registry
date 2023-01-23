namespace AssociationRegistry.Test.Admin.Api.CommandHandler.When_handling_a_WijzigBasisgegevensCommand.Given_A_WijzigBasisgegevensCommand;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using FluentAssertions;
using VCodes;
using Vereniging;
using Vereniging.WijzigBasisgegevens;
using Xunit;

public class With_A_Korte_Beschrijving
{
    private const string VCodeValue = "V0001001";
    private readonly VerenigingRepositoryMock.InvocationSave _invocationSave;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private const string NieuweKorteBeschrijving = "De nieuwe korte beschrijving";

    public With_A_Korte_Beschrijving()
    {
        var commandHandler = new WijzigBasisgegevensCommandHandler();
        var command = new WijzigBasisgegevensCommand(VCodeValue, KorteBeschrijving: NieuweKorteBeschrijving);
        var fixture = new Fixture();

        var commandMetadata = fixture.Create<CommandMetadata>();

        var naam = "GRUB";
        var datumLaatsteAanpassing = new DateOnly(2023, 1, 1);
        var vereniging = new Vereniging();
        vereniging.Apply(
            new VerenigingWerdGeregistreerd(
                VCodeValue, naam, null, null, null, null, null, null, datumLaatsteAanpassing));

        var verenigingRepositoryMock = new VerenigingRepositoryMock(
            vereniging);
        _verenigingRepositoryMock = verenigingRepositoryMock;
        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock).GetAwaiter().GetResult();

        _invocationSave = _verenigingRepositoryMock.InvocationsSave.Single();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.InvocationsLoad.Should().BeEquivalentTo(
            new[] { new VerenigingRepositoryMock.InvocationLoad(VCode.Create(VCodeValue)) },
            options => options.WithStrictOrdering());
    }

    [Fact]
    public void Then_A_KorteNaamWerdGewijzigd_Event_Is_Saved()
    {
        _invocationSave.Vereniging.UncommittedEvents.Should().BeEquivalentTo(
            new [] { new KorteBeschrijvingWerdGewijzigd(VCodeValue, NieuweKorteBeschrijving )},
            options => options.WithStrictOrdering() );
    }
}
