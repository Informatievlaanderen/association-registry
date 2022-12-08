// namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;
//
// using System.Net;
// using Be.Vlaanderen.Basisregisters.BasicApiProblem;
// using Fixtures;
// using FluentAssertions;
// using Newtonsoft.Json;
// using Xunit;
//
// public class Given_A_Request_With_An_Invalid_Startdatum_Fixture : JsonRequestAdminApiFixture
// {
//     public Given_A_Request_With_An_Invalid_Startdatum_Fixture() : base(
//         nameof(Given_A_Request_With_An_Invalid_Startdatum_Fixture),
//         "files.request.with_an_invalid_startdatum")
//     {
//     }
// }
//
// public class Given_A_Request_With_An_Invalid_Startdatum: IClassFixture<Given_A_Request_With_An_Invalid_Startdatum_Fixture>
// {
//     private readonly Given_A_Request_With_An_Invalid_Startdatum_Fixture _apiFixture;
//
//     public Given_A_Request_With_An_Invalid_Startdatum(Given_A_Request_With_An_Invalid_Startdatum_Fixture apiFixture)
//     {
//         _apiFixture = apiFixture;
//     }
//
//     [Fact]
//     public async Task Then_it_returns_a_badrequest_response()
//     {
//         var response = await _apiFixture.HttpClient.PostAsync("/v1/verenigingen", _apiFixture.Content);
//         response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//     }
//
//     [Fact]
//     public async Task Then_it_returns_a_problemdetails_response()
//     {
//         var response = await _apiFixture.HttpClient.PostAsync("/v1/verenigingen", _apiFixture.Content);
//
//         var responseContent = await response.Content.ReadAsStringAsync();
//
//         var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);
//         var expectedResponseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(GetJsonResponseBody());
//
//         responseContentObject.Should().BeEquivalentTo(
//             expectedResponseContentObject,
//             options => options
//                 .Excluding(info => info!.ProblemInstanceUri)
//                 .Excluding(info => info!.ProblemTypeUri));
//     }
//
//     private string GetJsonResponseBody()
//         => GetType()
//             .GetAssociatedResourceJson($"files.response.invalid_startdatum_validation_error");
// }
