// namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Sending_A_VerwijderVertegenwoordigerPersoonsgegevensMessage.Beheer.Historiek;
//
// using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
// using AssociationRegistry.Events;
// using AssociationRegistry.Test.E2E.Framework.AlbaHost;
// using AssociationRegistry.Test.E2E.Framework.ApiSetup;
// using AssociationRegistry.Test.E2E.Framework.Comparison;
// using AssociationRegistry.Test.E2E.Framework.Mappers;
// using AssociationRegistry.Test.E2E.Framework.TestClasses;
// using KellermanSoftware.CompareNetObjects;
// using Xunit;
//
// [Collection(nameof(VerwijderVertegenwoordigerPersoonsgegevensCollection))]
// public class Returns_Historiek : End2EndTest<HistoriekResponse>
// {
//     private readonly VerwijderVertegenwoordigerPersoonsgegevensTestContext _testContext;
//
//     public Returns_Historiek(VerwijderVertegenwoordigerPersoonsgegevensTestContext testContext) : base(testContext.ApiSetup)
//     {
//         _testContext = testContext;
//     }
//
//     public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup)
//         => await setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient, _testContext.VCode, headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));
//
//     [Fact]
//     public void With_VCode()
//     {
//         Response.VCode.ShouldCompare(_testContext.VCode);
//     }
//
//     [Fact]
//     public void With_Context()
//     {
//         Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
//     }
//
//     [Fact]
//     public void With_Naam_Werd_Gewijzigd_Gebeurtenis()
//     {
//         var naamWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(NaamWerdGewijzigd));
//
//         naamWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.NaamWerdGewijzigd(_testContext.VCode, _testContext.CommandRequest.Naam),
//                                         compareConfig: HistoriekComparisonConfig.Instance);
//     }
// }
