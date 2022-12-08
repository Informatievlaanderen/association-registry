// namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;
//
// using System.Net;
// using Fixtures;
// using FluentAssertions;
// using Framework.Helpers;
// using Xunit;
//
// public class Given_A_Request_With_Empty_KboNumber_Fixture : JsonRequestAdminApiFixture
// {
//     public Given_A_Request_With_Empty_KboNumber_Fixture() : base(
//         nameof(Given_A_Request_With_Empty_KboNumber_Fixture),
//         "files.request.with_empty_kbonummer")
//     {
//     }
// }
//
// public class Given_A_Request_With_Empty_KboNumber : IClassFixture<Given_A_Request_With_Empty_KboNumber_Fixture>
// {
//     private readonly Given_A_Request_With_Empty_KboNumber_Fixture _apiFixture;
//
//     public Given_A_Request_With_Empty_KboNumber(Given_A_Request_With_Empty_KboNumber_Fixture apiFixture)
//     {
//         _apiFixture = apiFixture;
//     }
//
//     [Fact]
//     public async Task Then_it_returns_an_ok_response()
//     {
//         var response = await _apiFixture.HttpClient.PostAsync("/v1/verenigingen", _apiFixture.Content);
//         response.StatusCode.Should().Be(HttpStatusCode.Accepted);
//     }
//
// }
