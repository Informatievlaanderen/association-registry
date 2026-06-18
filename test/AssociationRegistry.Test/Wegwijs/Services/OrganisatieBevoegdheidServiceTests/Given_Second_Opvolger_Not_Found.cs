namespace AssociationRegistry.Test.Wegwijs.Services.OrganisatieBevoegdheidServiceTests;

using Common.StubsMocksFakes.Wegwijs;
using FluentAssertions;
using Integrations.Wegwijs.Services;
using Xunit;

public class Given_Second_Opvolger_Not_Found
{
    [Fact]
    public async Task Then_Opvolgers_Should_Contain_First_Opvolger()
    {
        var stub = new IWegwijsClientMockStub()
           .SetupGemachtigdeOrganisatie("OVO001000", "OVO002000")
           .SetupThrowOrganisatieNietGevondenException("OVO002000");

        var sut = new OrganisatieBevoegdheidService(stub.Object);

        var opvolgers = await sut.GetOpvolgers("OVO001000");

        opvolgers.Should().BeEquivalentTo(["OVO002000"]);
    }
}
