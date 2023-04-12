namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using AssociationRegistry.Framework;
using Fakes;
using Framework;
using Vereniging.DuplicateDetection;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using ContactGegevens.Exceptions;
using FluentAssertions;
using Framework.MagdaMocks;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Vertegenwoordiger_With_Two_Duplicate_Contactgegevens
{
    private readonly CommandEnvelope<RegistreerVerenigingCommand> _commandEnvelope;
    private readonly RegistreerVerenigingCommandHandler _commandHandler;

    public With_A_Vertegenwoordiger_With_Two_Duplicate_Contactgegevens()
    {
        var fixture = new Fixture().CustomizeAll();
        var repositoryMock = new VerenigingRepositoryMock();
        var today = fixture.Create<DateOnly>();

        var contactgegeven = fixture.Create<RegistreerVerenigingCommand.Contactgegeven>();

        var command = fixture.Create<RegistreerVerenigingCommand>() with
        {
            Vertegenwoordigers = new[]
            {
                fixture.Create<RegistreerVerenigingCommand.Vertegenwoordiger>() with
                {
                    Contactgegevens = new[] { contactgegeven, contactgegeven },
                },
            },
        };

        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new RegistreerVerenigingCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            new MagdaFacadeEchoMock(),
            Mock.Of<IDuplicateDetectionService>(),
            new ClockStub(today));

        _commandEnvelope = new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_The_Result_Contains_The_Potential_Duplicates()
    {
        var method = () => _commandHandler.Handle(_commandEnvelope, CancellationToken.None);
        await method.Should().ThrowAsync<DuplicateContactgegeven>();
    }
}
