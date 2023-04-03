namespace AssociationRegistry.Test.Admin.Api.When_Removing_Contactgegeven.RequestHandling;

using AssociationRegistry.Admin.Api.Verenigingen.VerwijderContactgegeven;
using AssociationRegistry.Framework;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Vereniging;
using Vereniging.VerwijderContactgegevens;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Request
{
    private readonly VerwijderContactgegevenController _controller;
    private readonly Fixture _fixture;

    public With_Valid_Request()
    {
        _fixture = new Fixture().CustomizeAll();

        var messageBusMock = new Mock<IMessageBus>();
        messageBusMock.Setup(mb => mb.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<VerwijderContactgegevenCommand>>(), default, null))
            .ReturnsAsync(new Fixture().CustomizeAll().Create<CommandResult>());
        _controller = new VerwijderContactgegevenController(messageBusMock.Object);
    }

    [Fact]
    public async Task Then_it_returns_an_assceptedResponse()
    {
        var response = await _controller.Delete(
            _fixture.Create<string>(),
            _fixture.Create<int>());

        using (new AssertionScope())
        {
            response.Should().BeAssignableTo<IStatusCodeActionResult>();
            (response as IStatusCodeActionResult)!.StatusCode.Should().Be(202);
        }
    }
}
