namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using FluentAssertions;
using Framework;
using Kbo;
using Moq;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unsupported_Rechtsvorm
{
    private readonly RegistreerVerenigingUitKboCommandHandler _commandHandler;
    private readonly CommandEnvelope<RegistreerVerenigingUitKboCommand> _envelope;
    private readonly Mock<IMagdaGeefVerenigingService> _mockService;

    public With_An_Unsupported_Rechtsvorm()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _mockService = new Mock<IMagdaGeefVerenigingService>();

        _commandHandler = new RegistreerVerenigingUitKboCommandHandler(new VerenigingRepositoryMock(), new InMemorySequentialVCodeService(), _mockService.Object);
        _envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(fixture.Create<RegistreerVerenigingUitKboCommand>(), fixture.Create<CommandMetadata>());
    }

    [Theory]
    [InlineData("CVBA")]
    [InlineData("Niet juist")]
    public async Task Then_It_Throws_GeenGeldigeVerenigingInKbo(string rechtsvorm)
    {
        _mockService.Setup(s => s.GeefVereniging(It.IsAny<KboNummer>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (KboNummer kboNummer, string _, CancellationToken _) => VerenigingVolgensKboResult.GeldigeVereniging(
                    new VerenigingVolgensKbo
                    {
                        KboNummer = kboNummer,
                        Rechtsvorm = rechtsvorm,
                    }));

        var handle = () => _commandHandler
            .Handle(_envelope, CancellationToken.None);
        await handle.Should().ThrowAsync<GeenGeldigeVerenigingInKbo>();
    }
}
