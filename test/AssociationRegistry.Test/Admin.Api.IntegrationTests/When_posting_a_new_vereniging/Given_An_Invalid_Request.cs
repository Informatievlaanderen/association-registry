namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using System.Text;
using Fixtures;
using FluentAssertions;
using Framework.Helpers;
using Xunit;

[Collection(VerenigingAdminApiCollection.Name)]
public class Given_An_Invalid_Request
{
    private readonly VerenigingAdminApiFixture _apiFixture;

    public Given_An_Invalid_Request(VerenigingAdminApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_a_badrequest_response()
    {
        var content = GetJsonBody().AsJsonContent();
        var response = await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private string GetJsonBody()
        => GetType()
            .GetAssociatedResourceJson($"{nameof(Given_An_Invalid_Request)}_{nameof(Then_it_returns_a_badrequest_response)}");
}
