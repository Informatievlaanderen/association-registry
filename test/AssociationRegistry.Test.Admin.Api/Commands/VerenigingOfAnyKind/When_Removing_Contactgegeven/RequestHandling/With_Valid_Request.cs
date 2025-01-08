namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Removing_Contactgegeven.RequestHandling;

using Acties.Contactgegevens.VerwijderContactgegeven;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.VerwijderContactgegeven;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using Vereniging;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Request
{
    private readonly CommandMetadataProviderStub _initiatorProvider = new() { Initiator = "OVO000001" };
    private readonly VerwijderContactgegevenController _controller;
    private readonly Fixture _fixture;
    private readonly CommandResult _commandResult;

    public With_Valid_Request()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var messageBusMock = new Mock<IMessageBus>();
        _commandResult = new Fixture().CustomizeAdminApi().Create<CommandResult>();

        messageBusMock
           .Setup(mb => mb.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<VerwijderContactgegevenCommand>>(), default, null))
           .ReturnsAsync(_commandResult);

        _controller = new VerwijderContactgegevenController(messageBusMock.Object)
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    [Fact]
    public async Task Then_it_returns_an_assceptedResponse()
    {
        var response = await _controller.Delete(
            _fixture.Create<VCode>(),
            _fixture.Create<int>(),
            _initiatorProvider);

        using (new AssertionScope())
        {
            response.Should().BeAssignableTo<IStatusCodeActionResult>();
            (response as IStatusCodeActionResult)!.StatusCode.Should().Be(202);
        }
    }

    [Fact]
    public async Task Then_it_returns_a_sequence_header()
    {
        await _controller.Delete(
            _fixture.Create<VCode>(),
            _fixture.Create<int>(),
            _initiatorProvider);

        using (new AssertionScope())
        {
            _controller.Response.Headers[WellknownHeaderNames.Sequence].Should().BeEquivalentTo(_commandResult.Sequence.ToString());
        }
    }

    [Fact]
    public async Task Then_it_returns_a_etag_header()
    {
        await _controller.Delete(
            _fixture.Create<VCode>(),
            _fixture.Create<int>(),
            _initiatorProvider);

        using (new AssertionScope())
        {
            _controller.Response.GetTypedHeaders().ETag.Should()
                       .BeEquivalentTo(new EntityTagHeaderValue(new StringSegment($@"""{_commandResult.Version}"""), isWeak: true));
        }
    }
}
