// namespace AssociationRegistry.Test.KboSync;
//
// using AssociationRegistry.Framework;
// using AutoFixture;
// using Common.AutoFixture;
// using Kbo;
// using KboSyncLambda.SyncKbo;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Notifications;
// using Vereniging;
// using Xunit;
//
// public class Given_RegistreerInschrijving_Throws_Exception
// {
//     [Fact]
//     public async Task Then_ThrowsException()
//     {
//         var fixture = new Fixture().CustomizeAdminApi();
//         var magdaRegistreerInschrijvingService = new Mock<IMagdaRegistreerInschrijvingService>();
//         var magdaGeefVerenigingService = new Mock<IMagdaGeefVerenigingService>();
//         var verenigingsRepository = new Mock<IVerenigingsRepository>();
//         var kboNummer = fixture.Create<KboNummer>();
//
//         verenigingsRepository.Setup(x => x.Exists(kboNummer))
//                              .Returns(Task.FromResult(true));
//
//         magdaGeefVerenigingService
//            .Setup(x => x.GeefSyncVereniging(KboNummer.Create(kboNummer),
//                                             It.IsAny<CommandMetadata>(),
//                                             It.IsAny<CancellationToken>()))
//            .ReturnsAsync(VerenigingVolgensKboResult.GeldigeVereniging(new VerenigingVolgensKbo()));
//
//         verenigingsRepository.Setup(x => x.Load(KboNummer.Create(kboNummer), It.IsAny<CommandMetadata>()))
//                              .ThrowsAsync(new Exception());
//
//         magdaRegistreerInschrijvingService.Setup(x => x.)
//
//         var sut = new SyncKboCommandHandler(magdaRegistreerInschrijvingService.Object, magdaGeefVerenigingService.Object,
//                                             Mock.Of<INotifier>(), Mock.Of<ILogger<SyncKboCommandHandler>>());
//
//         await Assert
//            .ThrowsAsync<Exception>(() => sut.Handle(
//                                        new CommandEnvelope<SyncKboCommand>(new SyncKboCommand(kboNummer),
//                                                                            fixture.Create<CommandMetadata>()),
//                                        verenigingsRepository.Object));
//     }
// }
