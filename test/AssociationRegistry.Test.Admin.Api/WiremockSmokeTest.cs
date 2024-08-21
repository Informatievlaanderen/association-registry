namespace AssociationRegistry.Test.Admin.Api;

using FluentAssertions;
using System.Net;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class WiremockSmokeTest
{
    [Fact]
    public async Task With_A_Mapped_Uri_For_No_Exact_Match()
    {
        var client = new HttpClient();

        var response = await client.GetAsync(requestUri: "http://127.0.0.1:8080/v2/adresmatch?Gemeentenaam=Dendermonde");

        response.Should().HaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task With_A_Mapped_Uri_For_Exact_Match()
    {
        var client = new HttpClient();

        var response = await client.GetAsync(requestUri: "http://127.0.0.1:8080/v2/adresmatch?Gemeentenaam=Dendermonde&Postcode=9200");

        response.Should().HaveStatusCode(HttpStatusCode.OK);
    }

}
