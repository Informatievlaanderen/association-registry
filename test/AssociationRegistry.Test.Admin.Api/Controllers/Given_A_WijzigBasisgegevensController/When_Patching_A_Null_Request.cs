namespace AssociationRegistry.Test.Admin.Api.Controllers.Given_A_WijzigBasisgegevensController;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Xunit;

public class When_Patching_A_Null_Request
{
    private WijzigBasisgegevensController _controller;

    public When_Patching_A_Null_Request()
    {
        var messageBusMock = new MessageBusMock();
        _controller = new WijzigBasisgegevensController(messageBusMock, new AppSettings());
    }

    [Fact]
    public async Task Then_it_returns_a_BadRequestResponse()
    {
        var actionResult = await _controller.Patch(new WijzigBasisgegevensRequestValidator(), null, "V0001001", "M/\"1\"");
        var statusCodeResult = (IStatusCodeActionResult)actionResult;
        statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }
}
