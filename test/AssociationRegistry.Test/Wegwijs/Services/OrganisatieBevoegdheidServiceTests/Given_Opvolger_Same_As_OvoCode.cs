namespace AssociationRegistry.Test.Wegwijs.Services.OrganisatieBevoegdheidServiceTests;

using Common.StubsMocksFakes.Wegwijs;
using FluentAssertions;
using Integrations.Wegwijs.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class Given_Opvolger_Same_As_OvoCode
{
    [Fact]
    public async Task Then_Opvolgers_Should_Be_Empty()
    {
        var stub = new IWegwijsClientMockStub().SetupGemachtigdeOrganisatie("OVO001000", "OVO001000");
        var sut = new OrganisatieBevoegdheidService(stub.Object, NullLogger<OrganisatieBevoegdheidService>.Instance);

        var opvolgers = await sut.GetOpvolgers("OVO001000");

        opvolgers.Should().BeEmpty();
    }
}
