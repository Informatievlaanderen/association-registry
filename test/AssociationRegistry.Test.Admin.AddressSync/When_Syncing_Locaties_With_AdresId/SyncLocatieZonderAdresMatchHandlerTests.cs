// namespace AssociationRegistry.Test.Admin.AddressSync.Handlers;
//
// using AssociationRegistry.Admin.AddressSync;
// using AssociationRegistry.Admin.AddressSync.Fetchers;
// using AssociationRegistry.Admin.AddressSync.Handlers;
// using AssociationRegistry.Admin.Schema.Locaties;
// using AssociationRegistry.Grar.AdresMatch;
// using AssociationRegistry.Test.Common.AutoFixture;
// using AutoFixture;
// using CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
// using FluentAssertions;
// using Marten;
// using MartenDb.Store;
// using Microsoft.Extensions.Logging.Abstractions;
// using Moq;
//
// public class SyncLocatieZonderAdresMatchHandlerTests
// {
//     [Fact]
//     public async ValueTask Given_No_LocatieLookupDocumenten_Returns_Empty()
//     {
//         var store = await TestDocumentStoreFactory.CreateAsync(nameof(SyncLocatieZonderAdresMatchHandlerTests));
//         await using var session = store.LightweightSession();
//         var fetcher = new Mock<ITeSynchroniserenLocatiesZonderAdresMatchFetcher>();
//
//         fetcher.Setup(x => x.GetTeSynchroniserenLocatiesZonderAdresMatch(session, It.IsAny<CancellationToken>()))
//                .ReturnsAsync([]);
//
//         var commandHandler = new Mock<ProbeerAdresTeMatchenCommandHandler>(
//             Mock.Of<IAggregateSession>(),
//             Mock.Of<IAdresMatchService>(),
//             NullLogger<ProbeerAdresTeMatchenCommandHandler>.Instance);
//
//         var handler =
//             new SyncLocatieZonderAdresMatchHandler(NullLogger<SyncLocatieZonderAdresMatchHandler>.Instance,
//                                                    fetcher.Object,
//                                                    commandHandler.Object
//             );
//
//       var errors =  await handler.Handle(session, CancellationToken.None);
//       Assert.Empty(errors);
//     }
//
//     [Fact]
//     public async ValueTask
//         Given_1_LocatieZonderAdresMatchDocumenten_And_1_Locatie_Returns_Called_MatchCommandHandler_Once()
//     {
//         var store = await TestDocumentStoreFactory.CreateAsync(nameof(SyncLocatieZonderAdresMatchHandlerTests));
//         await using var session = store.LightweightSession();
//         var fixture = new Fixture().CustomizeAdminApi();
//
//         var doc = fixture.Create<LocatieZonderAdresMatchDocument>()
//             with
//             {
//                 LocatieIds = [fixture.Create<int>()],
//             };
//
//         var fetcher = new Mock<ITeSynchroniserenLocatiesZonderAdresMatchFetcher>();
//
//         fetcher.Setup(x => x.GetTeSynchroniserenLocatiesZonderAdresMatch(session, It.IsAny<CancellationToken>()))
//                .ReturnsAsync([doc]);
//
//         var commandHandler = new Mock<IProbeerAdresTeMatchenCommandHandler>();
//
//         var handler = new SyncLocatieZonderAdresMatchHandler(
//             NullLogger<SyncLocatieZonderAdresMatchHandler>.Instance,
//             fetcher.Object,
//             commandHandler.Object
//         );
//
//         var errors =  await handler.Handle(session, CancellationToken.None);
//         Assert.Empty(errors);
//         commandHandler.Verify(
//             x => x.Handle(
//                 It.Is<ProbeerAdresTeMatchenCommand>(c => c.VCode == doc.VCode),
//                 It.IsAny<CancellationToken>()),
//             Times.Once);
//     }
//
//     [Fact]
//     public async ValueTask
//         Given_1_LocatieZonderAdresMatchDocumenten_And_3_Locatie_Returns_Called_MatchCommandHandler_3_Times()
//     {
//         var store = await TestDocumentStoreFactory.CreateAsync(nameof(SyncLocatieZonderAdresMatchHandlerTests));
//         await using var session = store.LightweightSession();
//         var fixture = new Fixture().CustomizeAdminApi();
//
//         var doc = fixture.Create<LocatieZonderAdresMatchDocument>()
//             with
//             {
//                 LocatieIds = [fixture.Create<int>(), fixture.Create<int>(), fixture.Create<int>()],
//             };
//
//         var fetcher = new Mock<ITeSynchroniserenLocatiesZonderAdresMatchFetcher>();
//
//         fetcher.Setup(x => x.GetTeSynchroniserenLocatiesZonderAdresMatch(session, It.IsAny<CancellationToken>()))
//                .ReturnsAsync([doc]);
//
//         var commandHandler = new Mock<IProbeerAdresTeMatchenCommandHandler>();
//
//         var handler = new SyncLocatieZonderAdresMatchHandler(
//             NullLogger<SyncLocatieZonderAdresMatchHandler>.Instance,
//             fetcher.Object,
//             commandHandler.Object
//         );
//
//         await handler.Handle(session, CancellationToken.None);
//
//         commandHandler.Verify(
//             x => x.Handle(
//                 It.Is<ProbeerAdresTeMatchenCommand>(c => c.VCode == doc.VCode),
//                 It.IsAny<CancellationToken>()),
//             Times.Exactly(3));
//     }
//
//     [Fact]
//     public async ValueTask
//         Given_2_LocatieZonderAdresMatchDocumenten_And_1_Locatie_Returns_Called_MatchCommandHandler_Once()
//     {
//         var store = await TestDocumentStoreFactory.CreateAsync(nameof(SyncLocatieZonderAdresMatchHandlerTests));
//         await using var session = store.LightweightSession();
//         var fixture = new Fixture().CustomizeAdminApi();
//
//         var locatieDocument1 = fixture.Create<LocatieZonderAdresMatchDocument>()
//             with
//             {
//                 LocatieIds = [fixture.Create<int>()],
//             };
//
//         var locatieDocument2 = fixture.Create<LocatieZonderAdresMatchDocument>()
//             with
//             {
//                 LocatieIds = [fixture.Create<int>()],
//             };
//
//         var fetcher = new Mock<ITeSynchroniserenLocatiesZonderAdresMatchFetcher>();
//
//         fetcher.Setup(x => x.GetTeSynchroniserenLocatiesZonderAdresMatch(session, It.IsAny<CancellationToken>()))
//                .ReturnsAsync([locatieDocument1, locatieDocument2]);
//
//         var commandHandler = new Mock<IProbeerAdresTeMatchenCommandHandler>();
//
//         var handler = new SyncLocatieZonderAdresMatchHandler(
//             NullLogger<SyncLocatieZonderAdresMatchHandler>.Instance,
//             fetcher.Object,
//             commandHandler.Object
//         );
//
//         await handler.Handle(session, CancellationToken.None);
//
//         commandHandler.Verify(
//             x => x.Handle(
//                 It.Is<ProbeerAdresTeMatchenCommand>(c => c.VCode == locatieDocument1.VCode),
//                 It.IsAny<CancellationToken>()),
//             Times.Once);
//
//         commandHandler.Verify(
//             x => x.Handle(
//                 It.Is<ProbeerAdresTeMatchenCommand>(c => c.VCode == locatieDocument2.VCode),
//                 It.IsAny<CancellationToken>()),
//             Times.Once);
//     }
//
//     [Fact]
//     public async ValueTask
//         Given_2_LocatieZonderAdresMatchDocumenten_And_3_Locatie_Returns_Called_MatchCommandHandler_6_Times()
//     {
//         var store = await TestDocumentStoreFactory.CreateAsync(nameof(SyncLocatieZonderAdresMatchHandlerTests));
//         await using var session = store.LightweightSession();
//         var fixture = new Fixture().CustomizeAdminApi();
//
//         var locatieZonderAdresMatchDocument1 = fixture.Create<LocatieZonderAdresMatchDocument>()
//             with
//             {
//                 LocatieIds = [fixture.Create<int>(), fixture.Create<int>(), fixture.Create<int>()],
//             };
//
//         var locatieZonderAdresMatchDocument2 = fixture.Create<LocatieZonderAdresMatchDocument>()
//             with
//             {
//                 LocatieIds = [fixture.Create<int>(), fixture.Create<int>(), fixture.Create<int>()],
//             };
//
//         var fetcher = new Mock<ITeSynchroniserenLocatiesZonderAdresMatchFetcher>();
//
//         fetcher.Setup(x => x.GetTeSynchroniserenLocatiesZonderAdresMatch(session, It.IsAny<CancellationToken>()))
//                .ReturnsAsync([locatieZonderAdresMatchDocument1, locatieZonderAdresMatchDocument2]);
//
//         var commandHandler = new Mock<IProbeerAdresTeMatchenCommandHandler>();
//
//         var handler = new SyncLocatieZonderAdresMatchHandler(
//             NullLogger<SyncLocatieZonderAdresMatchHandler>.Instance,
//             fetcher.Object,
//             commandHandler.Object
//         );
//
//         await handler.Handle(session, CancellationToken.None);
//
//         commandHandler.Verify(
//             x => x.Handle(
//                 It.Is<ProbeerAdresTeMatchenCommand>(c => c.VCode == locatieZonderAdresMatchDocument1.VCode),
//                 It.IsAny<CancellationToken>()),
//             Times.Exactly(3));
//
//         commandHandler.Verify(
//             x => x.Handle(
//                 It.Is<ProbeerAdresTeMatchenCommand>(c => c.VCode == locatieZonderAdresMatchDocument2.VCode),
//                 It.IsAny<CancellationToken>()),
//             Times.Exactly(3));
//     }
//
//     [Fact]
//     public async ValueTask
//         Given_2_LocatieZonderAdresMatchDocumenten_And_3_Locatie_With_3_Exceptions_Returns_3_Errors()
//     {
//         var store = await TestDocumentStoreFactory.CreateAsync(nameof(SyncLocatieZonderAdresMatchHandlerTests));
//         await using var session = store.LightweightSession();
//         var fixture = new Fixture().CustomizeAdminApi();
//         var locatieIds1 = new[] { 1, 2, 3 };
//         var locatieIds2 = new[] { 4, 5, 6 };
//
//         var locatieZonderAdresMatchDocument1 = fixture.Create<LocatieZonderAdresMatchDocument>() with
//         {
//             LocatieIds = locatieIds1,
//         };
//
//         var locatieZonderAdresMatchDocument2 = fixture.Create<LocatieZonderAdresMatchDocument>() with
//         {
//             LocatieIds = locatieIds2,
//         };
//
//         var fetcher = new Mock<ITeSynchroniserenLocatiesZonderAdresMatchFetcher>();
//
//         fetcher
//            .Setup(x => x.GetTeSynchroniserenLocatiesZonderAdresMatch(
//                       It.IsAny<IDocumentSession>(),
//                       It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new[] { locatieZonderAdresMatchDocument1, locatieZonderAdresMatchDocument2 });
//
//         var commandHandler = new Mock<IProbeerAdresTeMatchenCommandHandler>();
//
//         var callIndex = 0;
//         commandHandler
//            .Setup(x => x.Handle(It.IsAny<ProbeerAdresTeMatchenCommand>(), It.IsAny<CancellationToken>()))
//            .Returns<ProbeerAdresTeMatchenCommand, CancellationToken>((cmd, _) =>
//             {
//                 if (cmd.LocatieId == 2 || cmd.LocatieId == 4 || cmd.LocatieId == 5)
//                     throw new Exception();
//
//                 return Task.CompletedTask;
//             });
//
//         var handler = new SyncLocatieZonderAdresMatchHandler(
//             NullLogger<SyncLocatieZonderAdresMatchHandler>.Instance,
//             fetcher.Object,
//             commandHandler.Object
//         );
//
//         var errors = await handler.Handle(session, CancellationToken.None);
//
//         commandHandler.Verify(
//             x => x.Handle(It.IsAny<ProbeerAdresTeMatchenCommand>(), It.IsAny<CancellationToken>()),
//             Times.Exactly(6));
//
//         errors.Length.Should().Be(3);
//
//         errors.Should().BeEquivalentTo([
//             new AdressSyncError(locatieZonderAdresMatchDocument1.VCode, [2]),
//             new AdressSyncError(locatieZonderAdresMatchDocument2.VCode, [4]),
//             new AdressSyncError(locatieZonderAdresMatchDocument2.VCode, [5]),
//         ]);
//     }
// }
