namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;
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
    private readonly WijzigMaatschappelijkeZetelController _controller;

    public With_Invalid_ETag()
    {
        Mock<IMessageBus> messageBusMock = new();

        _controller = new WijzigMaatschappelijkeZetelController(messageBusMock.Object, new WijzigMaatschappelijkeZetelRequestValidator(),
                                                                new AppSettings())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    [Theory]
    [InlineData("Invalid eTag Value")]
    public void Then_it_invokes_with_a_correct_version_number(string eTagValue)
    {
        var method = async () =>
        {
            await _controller.Patch(
                vCode: "V0001001",
                locatieId: 1,
                new WijzigMaatschappelijkeZetelRequest
                    { Locatie = { Naam = "naam" } },
                new CommandMetadataProviderStub { Initiator = "OVO000001" },
                eTagValue);
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
