namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
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
    private readonly WijzigBasisgegevensController _controller;

    public With_Invalid_ETag()
    {
        Mock<IMessageBus> messageBusMock = new();
        _controller = new WijzigBasisgegevensController(messageBusMock.Object, new AppSettings())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }


    [Theory]
    [InlineData("Invalid eTag Value")]
    public void Then_it_invokes_with_a_correct_version_number(string eTagValue)
    {
        var method = async () =>
        {
            await _controller.Patch(
                new WijzigBasisgegevensRequestValidator(),
                new WijzigBasisgegevensRequest { KorteNaam = "Korte naam" },
                "V0001001",
                eTagValue);
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
