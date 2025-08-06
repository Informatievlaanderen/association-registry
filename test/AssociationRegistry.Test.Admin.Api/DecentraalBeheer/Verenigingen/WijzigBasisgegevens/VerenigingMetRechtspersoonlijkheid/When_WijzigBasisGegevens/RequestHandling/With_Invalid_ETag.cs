namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Vereniging;
using Wolverine;
using Xunit;

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
                Mock.Of<IWerkingsgebiedenService>(),

            eTagValue);
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
