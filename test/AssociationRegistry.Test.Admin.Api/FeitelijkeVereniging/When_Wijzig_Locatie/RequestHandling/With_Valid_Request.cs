﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie.RequestHandling;

using AssociationRegistry.Acties.WijzigLocatie;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Request
{
    private readonly WijzigLocatieController _controller;
    private readonly Fixture _fixture;
    private CommandResult _commandResult;

    public With_Valid_Request()
    {
        _fixture = new Fixture().CustomizeAll();

        var messageBusMock = new Mock<IMessageBus>();
        _commandResult = new Fixture().CustomizeAll().Create<CommandResult>();
        messageBusMock
            .Setup(mb => mb.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<WijzigLocatieCommand>>(), default, null))
            .ReturnsAsync(_commandResult);
        _controller = new WijzigLocatieController(messageBusMock.Object, new WijzigLocatieValidator())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    [Fact]
    public async Task Then_it_returns_an_acceptedResponse()
    {
        var response = await _controller.Patch(
            _fixture.Create<VCode>(),
            _fixture.Create<int>(),
            _fixture.Create<WijzigLocatieRequest>(),
            _fixture.Create<InitiatorProvider>());

        using (new AssertionScope())
        {
            response.Should().BeAssignableTo<IStatusCodeActionResult>();
            (response as IStatusCodeActionResult)!.StatusCode.Should().Be(202);
        }
    }

    [Fact]
    public async Task Then_it_returns_a_sequence_header()
    {
        await _controller.Patch(
            _fixture.Create<VCode>(),
            _fixture.Create<int>(),
            _fixture.Create<WijzigLocatieRequest>(),
            _fixture.Create<InitiatorProvider>());

        using (new AssertionScope())
        {
            _controller.Response.Headers[WellknownHeaderNames.Sequence].Should().BeEquivalentTo(_commandResult.Sequence.ToString());
        }
    }

    [Fact]
    public async Task Then_it_returns_a_etag_header()
    {
        await _controller.Patch(
            _fixture.Create<VCode>(),
            _fixture.Create<int>(),
            _fixture.Create<WijzigLocatieRequest>(),
            _fixture.Create<InitiatorProvider>());

        using (new AssertionScope())
        {
            _controller.Response.GetTypedHeaders().ETag.Should().BeEquivalentTo(new EntityTagHeaderValue(new StringSegment($@"""{_commandResult.Version}"""), true));
        }
    }
}
