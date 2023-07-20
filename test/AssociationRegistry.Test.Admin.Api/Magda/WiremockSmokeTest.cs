namespace AssociationRegistry.Test.Admin.Api.Magda;

using System.Net;
using FluentAssertions;
using Xunit;

public class WiremockSmokeTest
{
    [Fact]
    public async Task With_A_Mapped_Uri()
    {
        var client = new HttpClient();

        var response = await client.PostAsync("http://localhost:8080/RegistreerInschrijvingDienst-02.01/soap/WebService", new StringContent(""));

        response.Should().HaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task With_A_Non_Mapped_Uri()
    {
        var client = new HttpClient();

        var response = await client.PostAsync("http://localhost:8080/DITBESTAATNIET-02.01/soap/WebService", new StringContent(""));

        response.Should().NotHaveStatusCode(HttpStatusCode.OK);
    }
}
