namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using FluentAssertions;
using Framework;
using Kbo;
using ResultNet;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_VerenigingVolgensKbo
{
    private readonly RegistreerVerenigingUitKboCommandHandler _commandHandler;
    private readonly CommandEnvelope<RegistreerVerenigingUitKboCommand> _envelope;

    public With_An_Unknown_VerenigingVolgensKbo()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new RegistreerVerenigingUitKboCommandHandler(new VerenigingRepositoryMock(), new InMemorySequentialVCodeService(),
                                                                       new MagdaGeefVerenigingNumberNotFoundMagdaGeefVerenigingService());

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

public class MagdaGeefVerenigingNumberNotFoundMagdaGeefVerenigingService : IMagdaGeefVerenigingService
{
    public Task<Result<VerenigingVolgensKbo>> GeefVereniging(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
        => Task.FromResult(VerenigingVolgensKboResult.GeenGeldigeVereniging);
}
