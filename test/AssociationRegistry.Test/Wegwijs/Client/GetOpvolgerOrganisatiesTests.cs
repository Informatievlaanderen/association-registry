namespace AssociationRegistry.Test.Wegwijs.Client;

using DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Wegwijs;
using FluentAssertions;
using Integrations.Wegwijs.Clients;
using Xunit;

public class GetOpvolgerOrganisatiesTests
{
    private readonly WegwijsClient _wegwijsClient;

    public GetOpvolgerOrganisatiesTests()
    {
        var client = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1:8080") };
        _wegwijsClient = new WegwijsClient(client);
    }

    [Fact]
    public async Task Without_Opvolger_Then_Returns_Empty_List()
    {
        var opvolgers = await _wegwijsClient.GetOpvolgerOrganisaties("OVO001000");

        opvolgers.Should().BeEmpty();
    }

    [Fact]
    public async Task With_Single_Opvolger_Then_Returns_List_With_One_OvoCode()
    {
        var opvolgers = await _wegwijsClient.GetOpvolgerOrganisaties("OVO002214");

        opvolgers.Should().ContainSingle().Which.Should().Be("OVO008382");
    }

    [Fact]
    public async Task With_Chained_Opvolgers_Then_Returns_Full_Chain()
    {
        var opvolgers = await _wegwijsClient.GetOpvolgerOrganisaties("OVO_CHAIN_START");

        opvolgers.Should().ContainInOrder("OVO_CHAIN_MIDDLE", "OVO_CHAIN_END");
    }

    [Fact]
    public async Task Unknown_OvoCode_Then_Returns_Empty_List()
    {
        var opvolgers = await _wegwijsClient.GetOpvolgerOrganisaties("404");

        opvolgers.Should().BeEmpty();
    }

    [Fact]
    public async Task With_Internal_ServerError_Then_Throws_WegwijsException()
    {
        await Assert.ThrowsAsync<WegwijsException>(() => _wegwijsClient.GetOpvolgerOrganisaties("500"));
    }
}
