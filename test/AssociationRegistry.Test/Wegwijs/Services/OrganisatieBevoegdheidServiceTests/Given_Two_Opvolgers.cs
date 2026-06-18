namespace AssociationRegistry.Test.Wegwijs.Services.OrganisatieBevoegdheidServiceTests;

using Common.StubsMocksFakes.Wegwijs;
using FluentAssertions;
using Integrations.Wegwijs.Services;
using Xunit;

public class Given_Two_Opvolgers
{
    [Fact]
    public async Task Then_Return_Two_Opvolgers()
    {
        var stub = new IWegwijsClientMockStub()
           .SetupGemachtigdeOrganisatie("OVO001000", "OVO002000")
           .SetupGemachtigdeOrganisatie("OVO002000", "OVO003000")
           .SetupGemachtigdeOrganisatieWithoutOpvolgers("OVO003000");

        var sut = new OrganisatieBevoegdheidService(stub.Object);

        var opvolgers = await sut.GetOpvolgers("OVO001000");

        opvolgers.Should().BeEquivalentTo(["OVO002000", "OVO003000"]);
    }
}
