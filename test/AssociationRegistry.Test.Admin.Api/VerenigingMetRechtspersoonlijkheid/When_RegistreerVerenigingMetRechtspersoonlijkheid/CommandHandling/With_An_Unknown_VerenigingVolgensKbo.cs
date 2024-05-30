namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using FluentAssertions;
using Framework;
using Marten;
using Microsoft.Extensions.Logging;
using Moq;
using ResultNet;
using Vereniging.Exceptions;
using Wolverine.Marten;
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

    }

    [Fact]
    public async Task Then_It_Throws_GeenGeldigeVerenigingInKbo()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var commandHandlerLogger = _loggerFactory.CreateLogger<RegistreerVerenigingUitKboCommandHandler>();

        var commandHandler = new RegistreerVerenigingUitKboCommandHandler();

        var envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(fixture.Create<RegistreerVerenigingUitKboCommand>(),
                                                                              fixture.Create<CommandMetadata>());

        var handle = () => commandHandler
           .Handle(envelope,
                   _verenigingRepositoryMock,
                   _vCodeService,
                   new MagdaGeefVerenigingNumberNotFoundServiceMock(),
                   new MagdaRegistreerInschrijvingServiceMock(Result.Success()),
                   commandHandlerLogger
                 , CancellationToken.None);

        await handle.Should().ThrowAsync<GeenGeldigeVerenigingInKbo>();
    }
}
