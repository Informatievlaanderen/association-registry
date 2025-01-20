namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Contactgegeven.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using AutoFixture;
using Common.AutoFixture;
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
    private readonly VoegContactgegevenToeController _controller;
    private readonly Fixture _fixture;

    public With_Invalid_ETag()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        var messageBusMock = new Mock<IMessageBus>();

        _controller = new VoegContactgegevenToeController(
                messageBusMock.Object,
                new VoegContactgegevenToeValidator(),
                new AppSettings()
                {
                    BaseUrl = "https://beheer.verenigingen.vlaanderen.be",
                })
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
                _fixture.Create<VoegContactgegevenToeRequest>(),
                _fixture.Create<CommandMetadataProviderStub>(),
                eTagValue);
        };

        method.Should().ThrowAsync<IfMatchParser.EtagHeaderIsInvalidException>();
    }
}
