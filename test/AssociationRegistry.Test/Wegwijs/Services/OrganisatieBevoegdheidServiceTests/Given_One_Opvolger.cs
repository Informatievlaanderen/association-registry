namespace AssociationRegistry.Test.Wegwijs.Services.OrganisatieBevoegdheidServiceTests;

using Common.StubsMocksFakes.Wegwijs;
using FluentAssertions;
using Integrations.Wegwijs.Services;
using Xunit;

public class Given_One_Opvolger
{
    [Fact]
    public async Task Then_Return_One_Opvolger()
    {
        var stub = new IWegwijsClientMockStub()
           .SetupGemachtigdeOrganisatie("OVO001000", "OVO002000")
           .SetupGemachtigdeOrganisatieWithoutOpvolgers("OVO002000");

        var sut = new OrganisatieBevoegdheidService(stub.Object);

        var opvolgers = await sut.GetOpvolgers("OVO001000");

        opvolgers.Should().BeEquivalentTo(["OVO002000"]);
    }
}
