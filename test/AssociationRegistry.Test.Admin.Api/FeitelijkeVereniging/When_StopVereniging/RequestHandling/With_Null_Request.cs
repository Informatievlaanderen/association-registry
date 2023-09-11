namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_StopVereniging.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Stop;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging;
using Fakes;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Request
{
    private readonly StopVerenigingController _controller;

    public With_Null_Request()
    {
        var messageBusMock = new MessageBusMock();
        _controller = new StopVerenigingController(messageBusMock, new AppSettings(), new StopVerenigingRequestValidator());
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Post(
                null,
                "V0001001",
                new CommandMetadataProviderStub { Initiator = "OVO0001001"},
                "M/\"1\""));
    }
}
