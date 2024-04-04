namespace AssociationRegistry.Test.Admin.Api.Grar;

using FluentAssertions;
using System.Net;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class WiremockSmokeTest
{
    [Fact]
    public async Task With_A_Mapped_Uri()
    {
        var client = new HttpClient();

        var response = await client.GetAsync(requestUri: "http://localhost:8080/v2/adresmatch");

        response.Should().HaveStatusCode(HttpStatusCode.OK);
    }
}
