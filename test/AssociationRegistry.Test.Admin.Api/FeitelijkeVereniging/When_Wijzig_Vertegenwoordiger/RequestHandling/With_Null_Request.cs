namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Vertegenwoordiger.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger;
using Framework;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Request
{
    private readonly WijzigVertegenwoordigerController _controller;

    public With_Null_Request()
    {
        var messageBus = Mock.Of<IMessageBus>();
        _controller = new WijzigVertegenwoordigerController(messageBus, new ValidatorStub<WijzigVertegenwoordigerRequest>());
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Patch(
                "V001001",
                1,
                null!,
                new CommandMetadataProviderStub { Initiator= "OVO0001000" },
                "M/\"1\""));
    }
}
