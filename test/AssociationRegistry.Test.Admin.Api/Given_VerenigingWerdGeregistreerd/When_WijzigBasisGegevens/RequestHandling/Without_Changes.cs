﻿namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using EventStore;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VCodes;
using Vereniging;
using Vereniging.WijzigBasisgegevens;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Without_Changes
{
    private readonly WijzigBasisgegevensController _controller;

    public Without_Changes()
    {
        var messageBusMock = new Mock<IMessageBus>();
        messageBusMock
            .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<WijzigBasisgegevensCommand>>(), default, null))
            .ReturnsAsync(CommandResult.Create(VCode.Create("V0001001"), StreamActionResult.Empty));
        _controller = new WijzigBasisgegevensController(messageBusMock.Object, new AppSettings())
            { ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() } };
    }

    [Fact]
    public async Task Then_it_returns_an_ok_response()
    {
        var result = await _controller.Patch(
            new WijzigBasisgegevensRequestValidator(),
            new WijzigBasisgegevensRequest() {Initiator = "V0001001", KorteNaam = "Korte naam"},
            "V0001001",
            "W/\"1\"");

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public void Then_it_returns_no_sequence_header()
    {
        _controller.Response.Headers.Should().NotContainKey(WellknownHeaderNames.Sequence);
    }

    [Fact]
    public void Then_it_returns_no_location_header()
    {
        _controller.Response.Headers.Should().NotContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
    }
}
