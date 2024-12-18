namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_StopVereniging.RequestHandling;

using Acties.StopVereniging;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Stop;
using AssociationRegistry.Admin.Api.Verenigingen.Stop.RequestModels;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using FluentAssertions;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Moq;
using Vereniging;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Without_Changes : IAsyncLifetime
{
    private readonly StopVerenigingController _controller;
    private IActionResult _result = null!;

    public Without_Changes()
    {
        var messageBusMock = new Mock<IMessageBus>();

        messageBusMock
           .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<StopVerenigingCommand>>(), default, null))
           .ReturnsAsync(CommandResult.Create(VCode.Create("V0001001"), StreamActionResult.Empty));

        _controller = new StopVerenigingController(messageBusMock.Object, new AppSettings(), new StopVerenigingRequestValidator())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async Task InitializeAsync()
    {
        _result = await _controller.Post(
            new StopVerenigingRequest
                { Einddatum = new DateOnly() },
            vCode: "V0001001",
            new CommandMetadataProviderStub { Initiator = "OVO0001001" },
            ifMatch: "W/\"1\"");
    }

    [Fact]
    public void Then_it_returns_an_ok_response()
    {
        _result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public void Then_it_returns_no_sequence_header()
    {
        _controller.Response.Headers.Should().NotContainKey(WellknownHeaderNames.Sequence);
    }

    [Fact]
    public void Then_it_returns_no_location_header()
    {
        _controller.Response.Headers.Should().NotContainKey(HeaderNames.Location);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
