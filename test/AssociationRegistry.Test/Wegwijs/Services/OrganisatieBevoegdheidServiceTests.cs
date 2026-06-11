namespace AssociationRegistry.Test.Wegwijs.Services;

using DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using FluentAssertions;
using Integrations.Wegwijs.Clients;
using Integrations.Wegwijs.Services;
using Moq;
using Resources;
using Xunit;

public class OrganisatieBevoegdheidServiceTests
{
    private readonly Mock<IWegwijsClient> _clientMock;
    private readonly OrganisatieBevoegdheidService _service;

    public OrganisatieBevoegdheidServiceTests()
    {
        _clientMock = new Mock<IWegwijsClient>();
        _service = new OrganisatieBevoegdheidService(_clientMock.Object);
    }

    [Fact]
    public async Task With_Same_OvoCode_Then_No_Api_Call_Is_Made()
    {
        var result = await _service.IsGemachtigdeOrganisatie("OVO001000", "OVO001000");

        result.Should().BeEmpty();
        _clientMock.Verify(
            c => c.GetOpvolgerOrganisaties(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task With_Initiator_From_OpvolgerOrganisatie_Then_Returns_Opvolgers()
    {
        _clientMock
            .Setup(c => c.GetOpvolgerOrganisaties("OVO001000", It.IsAny<CancellationToken>()))
            .ReturnsAsync(["OVO002000"]);

        var result = await _service.IsGemachtigdeOrganisatie("OVO002000", "OVO001000");

        result.Should().ContainSingle().Which.Should().Be("OVO002000");
    }

    [Fact]
    public async Task With_Initiator_As_Indirecte_Opvolger_Then_Returns_Opvolgers()
    {
        _clientMock
            .Setup(c => c.GetOpvolgerOrganisaties("OVO001000", It.IsAny<CancellationToken>()))
            .ReturnsAsync(["OVO002000", "OVO003000"]);

        var result = await _service.IsGemachtigdeOrganisatie("OVO003000", "OVO001000");

        result.Should().ContainInOrder("OVO002000", "OVO003000");
    }

    [Fact]
    public async Task With_Initiator_Not_In_Opvolgers_Then_Throws_GiIsNietBevoegd()
    {
        _clientMock
            .Setup(c => c.GetOpvolgerOrganisaties("OVO001000", It.IsAny<CancellationToken>()))
            .ReturnsAsync(["OVO002000"]);

        var exception = await Assert.ThrowsAsync<GiIsNietBevoegd>(async () =>
            await _service.IsGemachtigdeOrganisatie("OVO999999", "OVO001000")
        );

        exception.Message.Should().Be(ExceptionMessages.GiIsNietBevoegd);
    }

    [Fact]
    public async Task With_No_Opvolgers_And_Different_OvoCode_Then_Throws_GiIsNietBevoegd()
    {
        _clientMock.Setup(c => c.GetOpvolgerOrganisaties("OVO001000", It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var exception = await Assert.ThrowsAsync<GiIsNietBevoegd>(async () =>
            await _service.IsGemachtigdeOrganisatie("OVO999999", "OVO001000")
        );

        exception.Message.Should().Be(ExceptionMessages.GiIsNietBevoegd);
    }
}
