namespace AssociationRegistry.Test.Admin.Api.When_Adding_Vertegenwoordiger.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VoegVertegenwoordigerToe;
using Framework;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Invalid_ETag
{
    private readonly VoegVertegenwoordigerToeController _controller;
    private readonly Fixture _fixture;

    public With_Invalid_ETag()
    {
        _fixture = new Fixture().CustomizeAll();
        var messageBusMock = new Mock<IMessageBus>();
        _controller = new VoegVertegenwoordigerToeController(messageBusMock.Object, new VoegVertegenwoordigerToeValidator())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }


    [Theory]
    [InlineData("Invalid eTag Value")]
    public void Then_it_throws_an_exception(string eTagValue)
    {
        var method = async () =>
        {
            await _controller.Post(
                _fixture.Create<string>(),
                _fixture.Create<VoegVertegenwoordigerToeRequest>(),
                eTagValue);
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
