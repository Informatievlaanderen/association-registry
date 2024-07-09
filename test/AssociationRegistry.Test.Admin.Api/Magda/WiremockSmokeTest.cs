namespace AssociationRegistry.Test.Admin.Api.Magda;

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

        var response = await client.PostAsync(requestUri: "http://127.0.0.1:8080/GeefOndernemingDienst-02.00/soap/WebService",
                                              new StringContent(""));

        response.Should().HaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task With_A_Non_Mapped_Uri()
    {
        var client = new HttpClient();

        var response = await client.PostAsync(requestUri: "http://127.0.0.1:8080/DITBESTAATNIET-02.01/soap/WebService",
                                              new StringContent(""));

        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
}
