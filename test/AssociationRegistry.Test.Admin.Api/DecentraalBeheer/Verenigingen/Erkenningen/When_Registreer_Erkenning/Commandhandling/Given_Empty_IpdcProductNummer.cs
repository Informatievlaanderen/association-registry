// namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling;
//
// using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
// using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
// using AssociationRegistry.Framework;
// using AutoFixture;
// using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
// using Common.AutoFixture;
// using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
// using Common.StubsMocksFakes.VerenigingsRepositories;
// using FluentAssertions;
// using Resources;
// using Xunit;
//
// public class Given_Empty_IpdcProductNummer
// {
//     private readonly RegistreerErkenningCommandHandler _commandHandler;
//     private readonly Fixture _fixture;
//     private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
//
//     public Given_Empty_IpdcProductNummer()
//     {
//         _fixture = new Fixture().CustomizeAdminApi();
//
//         _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
//         var aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());
//
//         _commandHandler = new RegistreerErkenningCommandHandler(aggregateSessionMock);
//     }
//
//     [Fact]
//     public async ValueTask Then_An_IpdcProductNummerOntbreekt_Exception_Is_Thrown()
//     {
//         var command = _fixture.Create<RegistreerErkenningCommand>() with
//         {
//             VCode = _scenario.VCode,
//             Erkenning = _fixture.Create<TeRegistrerenErkenning>() with { IpdcProduct = null },
//         };
//
//         var exception = await Assert.ThrowsAsync<IpdcProductNummerOntbreekt>(async () =>
//             await _commandHandler.Handle(
//                 new CommandEnvelope<RegistreerErkenningCommand>(command, _fixture.Create<CommandMetadata>())
//             )
//         );
//
//         exception.Message.Should().Be(ExceptionMessages.IpdcProductNummerRequired);
//     }
// }
// TODO move to ipdcMiddleware
