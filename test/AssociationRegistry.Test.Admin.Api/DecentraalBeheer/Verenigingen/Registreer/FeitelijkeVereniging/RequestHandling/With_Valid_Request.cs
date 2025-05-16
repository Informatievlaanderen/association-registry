namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using ResultNet;
using Wolverine;
using Xunit;

public class With_Valid_Request
{
    private readonly RegistreerFeitelijkeVerenigingController _controller;
    private readonly Fixture _fixture;
    private readonly CommandResult _commandResult;

    public With_Valid_Request()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var messageBusMock = new Mock<IMessageBus>();

        _commandResult = _fixture.Create<CommandResult>();
        var result = Result.Success(_commandResult);

        messageBusMock
           .Setup(mb => mb.InvokeAsync<Result>(It.IsAny<CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>>(), default, null))
           .ReturnsAsync(result);

        _controller = new RegistreerFeitelijkeVerenigingController(
                messageBusMock.Object,
                new RegistreerFeitelijkeVerenigingRequestValidator(new Clock()),
                new AppSettings()
                {
                    BaseUrl = "https://beheer.verenigingen.vlaanderen.be",
                })
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    [Fact]
    public async Task Then_it_returns_an_acceptedResponse()
    {
        var response = await _controller.Post(
            _fixture.Create<RegistreerFeitelijkeVerenigingRequest>(),
            _fixture.Create<CommandMetadataProviderStub>(), Mock.Of<IWerkingsgebiedenService>());

        using (new AssertionScope())
        {
            response.Should().BeAssignableTo<IStatusCodeActionResult>();
            (response as IStatusCodeActionResult)!.StatusCode.Should().Be(202);
        }
    }

    [Fact]
    public async Task Then_it_returns_a_sequence_header()
    {
        await _controller.Post(
            _fixture.Create<RegistreerFeitelijkeVerenigingRequest>(),
            _fixture.Create<CommandMetadataProviderStub>(), Mock.Of<IWerkingsgebiedenService>());

        using (new AssertionScope())
        {
            _controller.Response.Headers[WellknownHeaderNames.Sequence].Should().BeEquivalentTo(_commandResult.Sequence.ToString());
        }
    }

    [Fact]
    public async Task Then_it_returns_a_etag_header()
    {
        await _controller.Post(
            _fixture.Create<RegistreerFeitelijkeVerenigingRequest>(),
            _fixture.Create<CommandMetadataProviderStub>(), Mock.Of<IWerkingsgebiedenService>());

        using (new AssertionScope())
        {
            _controller.Response.GetTypedHeaders().ETag.Should()
                       .BeEquivalentTo(new EntityTagHeaderValue(new StringSegment($@"""{_commandResult.Version}"""), isWeak: true));
        }
    }
}
