namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vertegenwoordigers.VoegVertegenwoordigerToe;
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
using Wolverine;
using Xunit;

public class With_Valid_Request
{
    private readonly VoegVertegenwoordigerToeController _controller;
    private readonly Fixture _fixture;
    private readonly EntityCommandResult _entityCommandResult;

    public With_Valid_Request()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var messageBusMock = new Mock<IMessageBus>();
        _entityCommandResult = new Fixture().CustomizeAdminApi().Create<EntityCommandResult>();

        messageBusMock
           .Setup(mb => mb.InvokeAsync<EntityCommandResult>(It.IsAny<CommandEnvelope<VoegVertegenwoordigerToeCommand>>(), default, null))
           .ReturnsAsync(_entityCommandResult);

        _controller = new VoegVertegenwoordigerToeController(
                messageBusMock.Object,
                new VoegVertegenwoordigerToeValidator(),
                new AppSettings()
                {
                    BaseUrl = "https://beheer.verenigingen.vlaanderen.be",
                })
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    [Fact]
    public async ValueTask Then_it_returns_an_acceptedResponse()
    {
        var response = await _controller.Post(
            _fixture.Create<VCode>(),
            _fixture.Create<VoegVertegenwoordigerToeRequest>(),
            _fixture.Create<CommandMetadataProviderStub>());

        using (new AssertionScope())
        {
            response.Should().BeAssignableTo<IStatusCodeActionResult>();
            (response as IStatusCodeActionResult)!.StatusCode.Should().Be(202);
        }
    }

    [Fact]
    public async ValueTask Then_it_returns_a_sequence_header()
    {
        await _controller.Post(
            _fixture.Create<VCode>(),
            _fixture.Create<VoegVertegenwoordigerToeRequest>(),
            _fixture.Create<CommandMetadataProviderStub>());

        using (new AssertionScope())
        {
            _controller.Response.Headers[WellknownHeaderNames.Sequence].Should().BeEquivalentTo(_entityCommandResult.Sequence.ToString());
        }
    }

    [Fact]
    public async ValueTask Then_it_returns_a_etag_header()
    {
        await _controller.Post(
            _fixture.Create<VCode>(),
            _fixture.Create<VoegVertegenwoordigerToeRequest>(),
            _fixture.Create<CommandMetadataProviderStub>());

        using (new AssertionScope())
        {
            _controller.Response.GetTypedHeaders().ETag.Should()
                       .BeEquivalentTo(new EntityTagHeaderValue(new StringSegment($@"""{_entityCommandResult.Version}"""), isWeak: true));
        }
    }

    [Fact]
    public async ValueTask Then_it_returns_an_location_header()
    {
        var response = await _controller.Post(
            _entityCommandResult.Vcode,
            _fixture.Create<VoegVertegenwoordigerToeRequest>(),
            _fixture.Create<CommandMetadataProviderStub>());

        using (new AssertionScope())
        {
            ((AcceptedResult)response)
               .Location.Should()
               .BeEquivalentTo(
                    $"https://beheer.verenigingen.vlaanderen.be/v1/verenigingen/{_entityCommandResult.Vcode}/vertegenwoordigers/{_entityCommandResult.EntityId}");
        }
    }
}
