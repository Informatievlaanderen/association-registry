namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.RequestHandling;

using AssociationRegistry.Admin.Api.Verenigingen.VoegContactGegevenToe;
using AssociationRegistry.Framework;
using Framework;
using Vereniging;
using Vereniging.AddContactgegevens;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Request
{
    private readonly VoegContactgegevenToeController _controller;
    private readonly Fixture _fixture;

    public With_Valid_Request()
    {
        _fixture = new Fixture().CustomizeAll();

        var messageBusMock = new Mock<IMessageBus>();
        messageBusMock.Setup(mb => mb.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<VoegContactgegevenToeCommand>>(), default, null))
            .ReturnsAsync(new Fixture().CustomizeAll().Create<CommandResult>());
        _controller = new VoegContactgegevenToeController(messageBusMock.Object, new VoegContactgegevenToeValidator());
    }

    [Fact]
    public async Task Then_it_returns_an_assceptedResponse()
    {
        var response = await _controller.Post(
            _fixture.Create<string>(),
            _fixture.Create<VoegContactgegevenToeRequest>());

        using (new AssertionScope())
        {
            response.Should().BeAssignableTo<IStatusCodeActionResult>();
            (response as IStatusCodeActionResult)!.StatusCode.Should().Be(202);
        }
    }
}
