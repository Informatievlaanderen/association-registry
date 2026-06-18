namespace AssociationRegistry.Test.Wegwijs.Services.OrganisatieBevoegdheidServiceTests;

using Common.StubsMocksFakes.Wegwijs;
using FluentAssertions;
using Integrations.Wegwijs.Services;
using Xunit;

public class Given_No_Opvolgers
{
    [Fact]
    public async Task Then_Opvolgers_Should_Be_Empty()
    {
        var stub = new IWegwijsClientMockStub().SetupGemachtigdeOrganisatieWithoutOpvolgers("OVO001000");
        var sut = new OrganisatieBevoegdheidService(stub.Object);

        var opvolgers = await sut.GetOpvolgers("OVO001000");

        opvolgers.Should().BeEmpty();
    }
}
