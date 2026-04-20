namespace AssociationRegistry.Test.Ipdc.IpdcClient;

using DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Ipdc;
using FluentAssertions;
using Integrations.Ipdc.Clients;
using Resources;
using Xunit;

/// <summary>
/// See wiremock for ipdc nummers
/// </summary>
public class IpdcClientTests
{
    private IpdcClient _ipdcClient;

    public IpdcClientTests()
    {
        var client = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1:8080") };

        _ipdcClient = new IpdcClient(client);
    }

    [Fact]
    public async ValueTask Valid_IpdcNummer_Then_Returns_IpdcProductResponse()
    {
        var validIpdcProductNummer = "9";

        var response = await _ipdcClient.GetById(validIpdcProductNummer);

        response.Should().NotBeNull();
    }

    [Fact]
    public async ValueTask IpdcNummer_Not_Found_Then_Throws_OnbekendIpdcProductNummer()
    {
        var ipdcProductNummerNotFound = "404";

        var exception = await Assert.ThrowsAsync<OnbekendIpdcProductNummer>(() =>
            _ipdcClient.GetById(ipdcProductNummerNotFound)
        );

        exception
            .Message.Should()
            .Be(string.Format(ExceptionMessages.OnbekendIpdcProductNummer, ipdcProductNummerNotFound));
    }

    [Fact]
    public async ValueTask Bad_Request_From_Ipdc_Then_Throws_OngeldigIpdcProductNummer()
    {
        var ipdcProductNummerBadRequest = "400";

        var exception = await Assert.ThrowsAsync<OngeldigIpdcProductNummer>(() =>
            _ipdcClient.GetById(ipdcProductNummerBadRequest)
        );

        exception
            .Message.Should()
            .Be(string.Format(ExceptionMessages.OngeldigIpdcProductNummer, ipdcProductNummerBadRequest));
    }

    [Fact]
    public async ValueTask Internal_Server_Error_From_Ipdc_Then_Throws_IpdcException()
    {
        var ipdcProductNummerNotFound = "500";

        await Assert.ThrowsAsync<IpdcException>(() => _ipdcClient.GetById(ipdcProductNummerNotFound));
    }
}
