namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Removing_Contactgegeven.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VerwijderContactgegeven;
using AssociationRegistry.Test.Admin.Api.Framework;
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
    private readonly CommandMetadataProviderStub _initiatorProvider = new() { Initiator = "OVO000001" };
    private readonly VerwijderContactgegevenController _controller;
    private readonly Fixture _fixture;

    public With_Invalid_ETag()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        var messageBusMock = new Mock<IMessageBus>();

        _controller = new VerwijderContactgegevenController(messageBusMock.Object)
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    [Theory]
    [InlineData("Invalid eTag Value")]
    public void Then_it_throws_an_exception(string eTagValue)
    {
        var method = async () =>
        {
            await _controller.Delete(
                _fixture.Create<string>(),
                _fixture.Create<int>(),
                _initiatorProvider,
                eTagValue);
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
