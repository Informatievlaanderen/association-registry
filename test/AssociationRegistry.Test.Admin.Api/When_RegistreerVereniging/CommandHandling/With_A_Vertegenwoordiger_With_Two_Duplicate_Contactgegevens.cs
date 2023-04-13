namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using AssociationRegistry.Framework;
using Fakes;
using Framework;
using AutoFixture;
using DuplicateVerenigingDetection;
using FluentAssertions;
using Framework.MagdaMocks;
using Moq;
using Vereniging;
using Vereniging.Exceptions;
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

        var contactgegeven = fixture.Create<Contactgegeven>();

        var vertegenwoordiger = fixture.Create<Vertegenwoordiger>();
        vertegenwoordiger.Contactgegevens[0] = contactgegeven;
        vertegenwoordiger.Contactgegevens[1] = contactgegeven;

        var command = fixture.Create<RegistreerVerenigingCommand>() with
        {
            Vertegenwoordigers = new[]
            {
                vertegenwoordiger,
            },
        };

        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new RegistreerVerenigingCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            new MagdaFacadeEchoMock(),
            Mock.Of<IDuplicateVerenigingDetectionService>(),
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
