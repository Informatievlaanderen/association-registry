namespace AssociationRegistry.Test.Wegwijs.Services.OrganisatieBevoegdheidServiceTests;

using Common.StubsMocksFakes.Wegwijs;
using FluentAssertions;
using Integrations.Wegwijs.Services;
using Xunit;

public class Given_Third_Opvolger_Same_As_First_OvoCode
{
    [Fact]
    public async Task Then_Opvolgers_Should_Contain_First_And_Second_Opvolgers_Response()
    {
        var stub = new IWegwijsClientMockStub()
           .SetupGemachtigdeOrganisatie("OVO001000", "OVO002000")
           .SetupGemachtigdeOrganisatie("OVO002000", "OVO003000")
           .SetupGemachtigdeOrganisatie("OVO003000", "OVO001000");

        var sut = new OrganisatieBevoegdheidService(stub.Object);

        var opvolgers = await sut.GetOpvolgers("OVO001000");

        opvolgers.Should().BeEquivalentTo(["OVO002000", "OVO003000"]);
    }
}
