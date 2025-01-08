﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.RequestHandling;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Locaties.VoegLocatieToe;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using Vereniging;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Request
{
    private readonly VoegLocatieToeController _controller;
    private readonly Fixture _fixture;
    private readonly EntityCommandResult _entityCommandResult;

    public With_Valid_Request()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var messageBusMock = new Mock<IMessageBus>();
        _entityCommandResult = new Fixture().CustomizeAdminApi().Create<EntityCommandResult>();

        messageBusMock
           .Setup(mb => mb.InvokeAsync<EntityCommandResult>(It.IsAny<CommandEnvelope<VoegLocatieToeCommand>>(), default, null))
           .ReturnsAsync(_entityCommandResult);

        _controller = new VoegLocatieToeController(
                messageBusMock.Object,
                new VoegLocatieToeValidator(),
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
            _fixture.Create<VCode>(),
            _fixture.Create<VoegLocatieToeRequest>(),
            _fixture.Create<CommandMetadataProviderStub>());

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
            _fixture.Create<VCode>(),
            _fixture.Create<VoegLocatieToeRequest>(),
            _fixture.Create<CommandMetadataProviderStub>());

        using (new AssertionScope())
        {
            _controller.Response.Headers[WellknownHeaderNames.Sequence].Should().BeEquivalentTo(_entityCommandResult.Sequence.ToString());
        }
    }

    [Fact]
    public async Task Then_it_returns_a_etag_header()
    {
        await _controller.Post(
            _fixture.Create<VCode>(),
            _fixture.Create<VoegLocatieToeRequest>(),
            _fixture.Create<CommandMetadataProviderStub>());

        using (new AssertionScope())
        {
            _controller.Response.GetTypedHeaders().ETag.Should()
                       .BeEquivalentTo(new EntityTagHeaderValue(new StringSegment($@"""{_entityCommandResult.Version}"""), isWeak: true));
        }
    }

    [Fact]
    public async Task Then_it_returns_a_location_header()
    {
        var response = await _controller.Post(
            _entityCommandResult.Vcode,
            _fixture.Create<VoegLocatieToeRequest>(),
            _fixture.Create<CommandMetadataProviderStub>());

        using (new AssertionScope())
        {
            ((AcceptedResult)response)
               .Location.Should()
               .BeEquivalentTo(
                    $"https://beheer.verenigingen.vlaanderen.be/v1/verenigingen/{_entityCommandResult.Vcode}/locaties/{_entityCommandResult.EntityId}");
        }
    }
}
