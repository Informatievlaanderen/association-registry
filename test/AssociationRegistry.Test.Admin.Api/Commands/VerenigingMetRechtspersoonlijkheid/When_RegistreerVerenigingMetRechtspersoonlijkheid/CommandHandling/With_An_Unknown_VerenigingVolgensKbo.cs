namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using FluentAssertions;
using Framework;
using Framework.Fakes;
using Microsoft.Extensions.Logging;
using ResultNet;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_VerenigingVolgensKbo
{
    private readonly RegistreerVerenigingUitKboCommandHandler _commandHandler;
    private readonly CommandEnvelope<RegistreerVerenigingUitKboCommand> _envelope;
    private readonly LoggerFactory _loggerFactory;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_An_Unknown_VerenigingVolgensKbo()
    {
        _loggerFactory = new LoggerFactory();
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        var commandHandlerLogger = _loggerFactory.CreateLogger<RegistreerVerenigingUitKboCommandHandler>();

        _commandHandler = new RegistreerVerenigingUitKboCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            new MagdaGeefVerenigingNumberNotFoundServiceMock(),
            new MagdaRegistreerInschrijvingServiceMock(Result.Success()),
            commandHandlerLogger
        );

        _envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(fixture.Create<RegistreerVerenigingUitKboCommand>(),
                                                                           fixture.Create<CommandMetadata>());
    }

    [Fact]
    public async Task Then_It_Throws_GeenGeldigeVerenigingInKbo()
    {
        var handle = () => _commandHandler
           .Handle(_envelope, CancellationToken.None);

        await handle.Should().ThrowAsync<GeenGeldigeVerenigingInKbo>();
    }
}
