namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_StopVereniging.RequestHandling;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Stop;
using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using Framework;
using Framework.Fakes;
using Hosts.Configuration.ConfigurationBindings;
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
                request: null,
                vCode: "V0001001",
                new CommandMetadataProviderStub { Initiator = "OVO0001001" },
                ifMatch: "M/\"1\""));
    }
}
