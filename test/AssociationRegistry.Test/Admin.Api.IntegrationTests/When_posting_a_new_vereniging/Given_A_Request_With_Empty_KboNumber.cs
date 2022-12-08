// namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;
//
// using System.Net;
// using Fixtures;
// using FluentAssertions;
// using Framework.Helpers;
// using Xunit;
//
// [Collection(VerenigingAdminApiCollection.Name)]
// public class Given_A_Request_With_Empty_KboNumber
// {
//     private readonly VerenigingAdminApiFixture _apiFixture;
//
//     public Given_A_Request_With_Empty_KboNumber(VerenigingAdminApiFixture apiFixture)
//     {
//         _apiFixture = apiFixture;
//     }
//
//     [Fact]
//     public async Task Then_it_returns_an_ok_response()
//     {
//         var content = GetJsonRequestBody().AsJsonContent();
//         var response = await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);
//         response.StatusCode.Should().Be(HttpStatusCode.Accepted);
//     }
//
//     private string GetJsonRequestBody()
//         => GetType()
//             .GetAssociatedResourceJson($"files.request.with_empty_kbonummer");
//
// }
