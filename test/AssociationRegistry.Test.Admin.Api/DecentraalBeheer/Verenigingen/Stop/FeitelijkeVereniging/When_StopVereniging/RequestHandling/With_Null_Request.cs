namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Stop.FeitelijkeVereniging.When_StopVereniging.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Stop;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using Xunit;

public class With_Null_Request
{
    private readonly StopVerenigingController _controller;

    public With_Null_Request()
    {
        var messageBusMock = new MessageBusMock();
        _controller = new StopVerenigingController(messageBusMock, new StopVerenigingRequestValidator());
    }

    [Fact]
    public async ValueTask Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(async () =>
            await _controller.Post(
                request: null,
                vCode: "V0001001",
                new CommandMetadataProviderStub { Initiator = "OVO0001001" },
                ifMatch: "M/\"1\""
            )
        );
    }
}
