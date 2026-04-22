namespace AssociationRegistry.Test.Wegwijs.Client;

using DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Wegwijs;
using FluentAssertions;
using Integrations.Wegwijs.Clients;
using Resources;
using Xunit;

/// <summary>
/// See wiremock for ipdc nummers
/// </summary>
public class WegwijsClientTests
{
    private WegwijsClient _wegwijsClient;

    public WegwijsClientTests()
    {
        var client = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1:8080") };

        _wegwijsClient = new WegwijsClient(client);
    }

    [Fact]
    public async ValueTask Valid_OvoCode_Then_Returns_OrganisationResponse_With_Name()
    {
        var validOvoCode = "OVO001000";

        var response = await _wegwijsClient.GetOrganisationByOvoCode(validOvoCode);

        response.Name.Should().NotBeNull();
    }

    [Fact]
    public async ValueTask Unknown_OvoCode_Then_Throws_OrganisatieNietGevondenException()
    {
        var invalidOvoCode = "404";

        var exception = await Assert.ThrowsAsync<OrganisatieNietGevondenException>(() =>
            _wegwijsClient.GetOrganisationByOvoCode(invalidOvoCode)
        );

        exception
            .Message.Should()
            .Be(string.Format(ExceptionMessages.OrganisatieNietGevondenException, invalidOvoCode));
    }

    [Fact]
    public async ValueTask With_Internal_ServerError_Then_Throws_WegwijsException()
    {
        var internalServerErrorOvoCode = "500";

        await Assert.ThrowsAsync<WegwijsException>(() =>
            _wegwijsClient.GetOrganisationByOvoCode(internalServerErrorOvoCode)
        );
    }
}
