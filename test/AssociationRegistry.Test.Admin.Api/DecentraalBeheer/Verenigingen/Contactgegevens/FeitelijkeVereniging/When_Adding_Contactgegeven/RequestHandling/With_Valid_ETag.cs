﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Adding_Contactgegeven.RequestHandling;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using AssociationRegistry.DecentraalBeheer.Contactgegevens.VoegContactgegevenToe;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Wolverine;
using Xunit;

public class With_Valid_ETag : IAsyncLifetime
{
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly VoegContactgegevenToeController _toeController;
    private readonly Fixture _fixture;
    private const int ETagNumber = 1;

    public With_Valid_ETag()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _messageBusMock = new Mock<IMessageBus>();

        _messageBusMock
           .Setup(x => x.InvokeAsync<EntityCommandResult>(It.IsAny<CommandEnvelope<VoegContactgegevenToeCommand>>(), default, null))
           .ReturnsAsync(new Fixture().CustomizeAdminApi().Create<EntityCommandResult>());

        _toeController = new VoegContactgegevenToeController(
                _messageBusMock.Object,
                new VoegContactgegevenToeValidator(),
                new AppSettings()
                {
                    BaseUrl = "https://beheer.verenigingen.vlaanderen.be",
                })
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async ValueTask InitializeAsync()
    {
        await _toeController.Post(
            _fixture.Create<VCode>(),
            _fixture.Create<VoegContactgegevenToeRequest>(),
            _fixture.Create<CommandMetadataProviderStub>(),
            $"W/\"{ETagNumber}\"");
    }

    [Fact]
    public void Then_it_invokes_with_a_correct_version_number()
    {
        _messageBusMock.Verify(
            expression: messageBus =>
                messageBus.InvokeAsync<EntityCommandResult>(
                    It.Is<CommandEnvelope<VoegContactgegevenToeCommand>>(
                        env =>
                            env.Metadata.ExpectedVersion == ETagNumber),
                    default,
                    null),
            Times.Once);
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}
