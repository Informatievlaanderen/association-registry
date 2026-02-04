namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.When_Wijzig_ContactgegevenFromKbo.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Wolverine;
using Xunit;

public class With_Invalid_ETag
{
    private readonly WijzigContactgegevenController _controller;

    public With_Invalid_ETag()
    {
        Mock<IMessageBus> messageBusMock = new();

        _controller = new WijzigContactgegevenController(messageBusMock.Object, new WijzigContactgegevenValidator())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };
    }

    [Theory]
    [InlineData("Invalid eTag Value")]
    public void Then_it_throws(string eTagValue)
    {
        var method = async () =>
        {
            await _controller.Patch(
                vCode: "V0001001",
                contactgegevenId: 1,
                new WijzigContactgegevenRequest
                {
                    Contactgegeven = new TeWijzigenContactgegeven { Beschrijving = "Beschrijving" },
                },
                new CommandMetadataProviderStub { Initiator = "OVO000001" },
                eTagValue
            );
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
