namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using AssociationRegistry.Framework;
using Fakes;
using Framework;
using Vereniging.DuplicateDetection;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using Contactgegevens;
using Contactgegevens.Exceptions;
using FluentAssertions;
using Framework.MagdaMocks;
using Moq;
using Vertegenwoordigers;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Vertegenwoordiger_With_Two_Primair_Contactgegevens_Of_The_Same_Type
{
    private readonly CommandEnvelope<RegistreerVerenigingCommand> _commandEnvelope;
    private readonly RegistreerVerenigingCommandHandler _commandHandler;

    public With_A_Vertegenwoordiger_With_Two_Primair_Contactgegevens_Of_The_Same_Type()
    {
        var fixture = new Fixture().CustomizeAll();
        var repositoryMock = new VerenigingRepositoryMock();
        var today = fixture.Create<DateOnly>();

        var vertegenwoordiger = fixture.Create<Vertegenwoordiger>();
        vertegenwoordiger.Contactgegevens[0] =  Contactgegeven.Create(ContactgegevenType.Email, "test@example.org", fixture.Create<string>(), true);
        vertegenwoordiger.Contactgegevens[1] =  Contactgegeven.Create(ContactgegevenType.Email, "test2@example.org", fixture.Create<string>(), true);
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
            Mock.Of<IDuplicateDetectionService>(),
            new ClockStub(today));

        _commandEnvelope = new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_The_Result_Contains_The_Potential_Duplicates()
    {
        var method = () => _commandHandler.Handle(_commandEnvelope, CancellationToken.None);
        await method.Should().ThrowAsync<MultiplePrimaryContactgegevens>();
    }
}
