namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Wijzig_Contactgegeven.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven.RequestModels;
using AssociationRegistry.DecentraalBeheer.Contactgegevens.WijzigContactgegeven;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Without_Changes : IAsyncLifetime
{
    private readonly WijzigContactgegevenController _controller;
    private IActionResult _result = null!;

    public Without_Changes()
    {
        var messageBusMock = new Mock<IMessageBus>();

        messageBusMock
           .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<WijzigContactgegevenCommand>>(), default, null))
           .ReturnsAsync(CommandResult.Create(VCode.Create("V0001001"), StreamActionResult.Empty));

        _controller = new WijzigContactgegevenController(messageBusMock.Object, new InlineValidator<WijzigContactgegevenRequest>())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async ValueTask InitializeAsync()
    {
        _result = await _controller.Patch(
            vCode: "V0001001",
            contactgegevenId: 1,
            new WijzigContactgegevenRequest { Contactgegeven = new TeWijzigenContactgegeven() },
            new CommandMetadataProviderStub(),
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

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}
