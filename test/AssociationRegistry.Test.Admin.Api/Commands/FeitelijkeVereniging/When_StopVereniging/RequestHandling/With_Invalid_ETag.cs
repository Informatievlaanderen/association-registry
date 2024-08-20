namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_StopVereniging.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Stop;
using AssociationRegistry.Admin.Api.Verenigingen.Stop.RequestModels;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
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
    private readonly StopVerenigingController _controller;

    public With_Invalid_ETag()
    {
        Mock<IMessageBus> messageBusMock = new();

        _controller = new StopVerenigingController(messageBusMock.Object, new AppSettings(), new StopVerenigingRequestValidator())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    [Theory]
    [InlineData("Invalid eTag Value")]
    public void Then_it_invokes_with_a_correct_version_number(string eTagValue)
    {
        var method = async () =>
        {
            await _controller.Post(
                new StopVerenigingRequest(),
                vCode: "V0001001",
                new CommandMetadataProviderStub { Initiator = "OVO0001001" },
                eTagValue);
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
