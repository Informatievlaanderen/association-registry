// namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;
//
// using System.Net;
// using Be.Vlaanderen.Basisregisters.BasicApiProblem;
// using Fixtures;
// using FluentAssertions;
// using Framework.Helpers;
// using Newtonsoft.Json;
// using Xunit;
//
// public class Given_A_Request_With_Startdatum_In_The_Future_Fixture : AdminApiFixture
// {
//     public Given_A_Request_With_Startdatum_In_The_Future_Fixture() : base(
//         nameof(Given_A_Request_With_Startdatum_In_The_Future_Fixture))
//     {
//         var startdatum = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
//         Content = GetJsonRequestBody(startdatum).AsJsonContent();
//     }
//
//     public StringContent Content { get; }
//
//
//     private string GetJsonRequestBody(DateOnly startdatum)
//         => GetType()
//             .GetAssociatedResourceJson($"files.request.with_startdatum_in_future")
//             .Replace("{{vereniging.startdatum}}", startdatum.ToString("yyyy-MM-dd"));
// }
//
// public class Given_A_Request_With_Startdatum_In_The_Future : IClassFixture<Given_A_Request_With_Startdatum_In_The_Future_Fixture>
// {
//     private readonly Given_A_Request_With_Startdatum_In_The_Future_Fixture _apiFixture;
//
//
//     public Given_A_Request_With_Startdatum_In_The_Future(Given_A_Request_With_Startdatum_In_The_Future_Fixture apiFixture)
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
//             .GetAssociatedResourceJson($"files.response.startdatum_in_future_validation_error");
// }
