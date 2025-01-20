namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using FluentAssertions;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
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
                new WijzigBasisgegevensRequest { KorteBeschrijving = "Korte naam" },
                vCode: "V0001001",
                new CommandMetadataProviderStub { Initiator = "OVO000001" },
                eTagValue);
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
