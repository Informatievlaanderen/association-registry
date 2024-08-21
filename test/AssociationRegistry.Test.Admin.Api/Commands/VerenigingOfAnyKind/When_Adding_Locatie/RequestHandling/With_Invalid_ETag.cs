namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
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
    private readonly VoegLocatieToeController _controller;
    private readonly Fixture _fixture;

    public With_Invalid_ETag()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        var messageBusMock = new Mock<IMessageBus>();

        _controller = new VoegLocatieToeController(messageBusMock.Object, new VoegLocatieToeValidator())
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
                _fixture.Create<VoegLocatieToeRequest>(),
                new CommandMetadataProviderStub { Initiator = "OVO0001000" },
                eTagValue);
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
