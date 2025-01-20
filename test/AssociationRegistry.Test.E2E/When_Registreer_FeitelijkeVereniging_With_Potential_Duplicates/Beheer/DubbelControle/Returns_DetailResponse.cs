namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates.Beheer.DubbelControle;

using Admin.Api.Administratie.DubbelControle;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using FluentAssertions;
using Xunit;

// TODO: fix superadmin authorization in tests
// [Collection(FullBlownApiCollection.Name)]
// public class Returns_Explanation : End2EndTest<RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext, RegistreerFeitelijkeVerenigingRequest, DubbelControleResponse[]>
// {
//     public override Func<IApiSetup, DubbelControleResponse[]> GetResponse
//         => setup => setup.AdminApiHost.PostDubbelControle(Request);
//
//     public Returns_Explanation(RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext context): base(context)
//     {
//     }
//
//     [Fact]
//     public async Task WithScore()
//     {
//         Response.First().Score.Should().BePositive();
//         Response.First().Explanation.Should().NotBeNullOrEmpty();
//     }
// }
