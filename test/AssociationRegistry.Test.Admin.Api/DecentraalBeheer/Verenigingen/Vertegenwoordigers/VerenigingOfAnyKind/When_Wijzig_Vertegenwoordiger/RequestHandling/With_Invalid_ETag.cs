namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Wolverine;
using Xunit;

public class With_Invalid_ETag
{
    private readonly WijzigVertegenwoordigerController _controller;
    private readonly Fixture _fixture;

    public With_Invalid_ETag()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        var messageBusMock = new Mock<IMessageBus>();

        _controller = new WijzigVertegenwoordigerController(messageBusMock.Object, new ValidatorStub<WijzigVertegenwoordigerRequest>())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    [Theory]
    [InlineData("Invalid eTag Value")]
    public void Then_it_throws_an_exception(string eTagValue)
    {
        var method = async () =>
        {
            await _controller.Patch(
                _fixture.Create<string>(),
                _fixture.Create<int>(),
                _fixture.Create<WijzigVertegenwoordigerRequest>(),
                new CommandMetadataProviderStub { Initiator = "OVO0001000" },
                eTagValue);
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
