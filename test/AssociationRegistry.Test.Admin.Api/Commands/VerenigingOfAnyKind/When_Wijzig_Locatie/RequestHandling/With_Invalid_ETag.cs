namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Locatie.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AutoFixture;
using FluentAssertions;
using Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Invalid_ETag
{
    private readonly WijzigLocatieController _controller;
    private readonly Fixture _fixture;

    public With_Invalid_ETag()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        var messageBusMock = new Mock<IMessageBus>();

        _controller = new WijzigLocatieController(messageBusMock.Object, new WijzigLocatieRequestValidator())
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
                _fixture.Create<WijzigLocatieRequest>(),
                new CommandMetadataProviderStub { Initiator = "OVO0001000" },
                eTagValue);
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
