namespace AssociationRegistry.Test.Wegwijs.Services.OrganisatieBevoegdheidServiceTests;

using Common.StubsMocksFakes.Wegwijs;
using FluentAssertions;
using Integrations.Wegwijs.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class Given_Second_Opvolger_Same_As_OvoCode
{
    [Fact]
    public async Task Then_Opvolgers_Should_Contain_First_Opvolgers_Response()
    {
        var stub = new IWegwijsClientMockStub()
            .SetupGemachtigdeOrganisatie("OVO001000", "OVO002000")
            .SetupGemachtigdeOrganisatie("OVO002000", "OVO002000");

        var sut = new OrganisatieBevoegdheidService(stub.Object, NullLogger<OrganisatieBevoegdheidService>.Instance);

        var opvolgers = await sut.GetOpvolgers("OVO001000");

        opvolgers.Should().BeEquivalentTo(["OVO002000"]);
    }
}
