namespace AssociationRegistry.Test.KboSync;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Kbo;
using KboSyncLambda.SyncKbo;
using Microsoft.Extensions.Logging;
using Moq;
using Notifications;
using Vereniging;
using Xunit;

public class Given_VerenigingNietGekend
{
    [Fact]
    public async Task Then_Nothing()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var magdaRegistreerInschrijvingService = new Mock<IMagdaRegistreerInschrijvingService>();
        var magdaGeefVerenigingService = new Mock<IMagdaGeefVerenigingService>();
        var verenigingsRepository = new Mock<IVerenigingsRepository>();
        var kboNummer = fixture.Create<KboNummer>();

        verenigingsRepository.Setup(x => x.Exists(kboNummer))
                             .Returns(Task.FromResult(false));

        var sut = new SyncKboCommandHandler(magdaRegistreerInschrijvingService.Object, magdaGeefVerenigingService.Object,
                                            Mock.Of<INotifier>(), Mock.Of<ILogger<SyncKboCommandHandler>>());


        var actual = await sut.Handle(
            new CommandEnvelope<SyncKboCommand>(new SyncKboCommand(kboNummer), fixture.Create<CommandMetadata>()),
            verenigingsRepository.Object);

        actual.Should().BeNull();
    }
}
