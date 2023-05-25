﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.CommandHandling;

using Acties.VerenigingMetRechtspersoonlijkheid.WijzigBasisgegevens;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;
using Framework;
using AutoFixture;
using Xunit;
using Xunit.Categories;

//[UnitTest]
// public class With_The_Same_KorteBeschrijving
// {
//     private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
//     private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
//
//     public With_The_Same_KorteBeschrijving()
//     {
//         _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
//         _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());
//
//         var fixture = new Fixture().CustomizeAll();
//         var command = new WijzigBasisgegevensCommand(_scenario.VCode, KorteBeschrijving: _scenario.FeitelijkeVerenigingWerdGeregistreerd.KorteBeschrijving);
//         var commandMetadata = fixture.Create<CommandMetadata>();
//         var commandHandler = new WijzigBasisgegevensCommandHandler();
//
//         commandHandler.Handle(
//             new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
//             _verenigingRepositoryMock).GetAwaiter().GetResult();
//     }
//
//     [Fact]
//     public void Then_The_Correct_Vereniging_Is_Loaded_Once()
//     {
//         _verenigingRepositoryMock.ShouldHaveLoaded(_scenario.VCode);
//     }
//
//     [Fact]
//     public void Then_No_Event_Is_Saved()
//     {
//         _verenigingRepositoryMock.ShouldNotHaveSaved<KorteBeschrijvingWerdGewijzigd>();
//     }
// }
