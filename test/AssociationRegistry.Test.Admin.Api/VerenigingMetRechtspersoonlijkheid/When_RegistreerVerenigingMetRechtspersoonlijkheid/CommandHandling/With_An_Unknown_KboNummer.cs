namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.CommandHandling;

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
public class With_An_Unknown_KboNummer
{
    private readonly RegistreerVerenigingUitKboCommandHandler _commandHandler;
    private readonly CommandEnvelope<RegistreerVerenigingUitKboCommand> _envelope;

    public With_An_Unknown_KboNummer()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new RegistreerVerenigingUitKboCommandHandler(new VerenigingRepositoryMock(), new InMemorySequentialVCodeService(), new MagdaGeefVerenigingNumberNotFoundMagdaGeefVerenigingService());
        _envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(fixture.Create<RegistreerVerenigingUitKboCommand>(), fixture.Create<CommandMetadata>());
    }

    [Fact]
    public async Task Then_It_Throws_UnknownKboNummer()
    {
        var handle = () => _commandHandler
            .Handle(_envelope, CancellationToken.None);
       await handle.Should().ThrowAsync<GeenGeldigeVerenigingInKbo>();
    }
}

public class MagdaGeefVerenigingNumberNotFoundMagdaGeefVerenigingService : IMagdaGeefVerenigingService
{
    public Task<Result> GeefVereniging(KboNummer kboNummer, string initiator, CancellationToken cancellationToken)
        => Task.FromResult<Result>(VerenigingVolgensKboResult.GeenGeldigeVereniging);
}
