﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Contactgegeven.RequestHandling;

using Acties.VoegContactgegevenToe;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;
using AssociationRegistry.Framework;
using Framework;
using Vereniging;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_ETag : IAsyncLifetime
{
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly VoegContactgegevenToeController _toeController;
    private readonly Fixture _fixture;
    private const int ETagNumber = 1;

    public With_Valid_ETag()
    {
        _fixture = new Fixture().CustomizeAll();
        _messageBusMock = new Mock<IMessageBus>();
        _messageBusMock
            .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<VoegContactgegevenToeCommand>>(), default, null))
            .ReturnsAsync(new Fixture().CustomizeAll().Create<CommandResult>());

        _toeController = new VoegContactgegevenToeController(_messageBusMock.Object, new VoegContactgegevenToeValidator())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async Task InitializeAsync()
    {
        await _toeController.Post(
            _fixture.Create<VCode>(),
            _fixture.Create<VoegContactgegevenToeRequest>(),
            _fixture.Create<string>(),
            $"W/\"{ETagNumber}\"");
    }

    [Fact]
    public void Then_it_invokes_with_a_correct_version_number()
    {
        _messageBusMock.Verify(
            messageBus =>
                messageBus.InvokeAsync<CommandResult>(
                    It.Is<CommandEnvelope<VoegContactgegevenToeCommand>>(
                        env =>
                            env.Metadata.ExpectedVersion == ETagNumber),
                    default,
                    null),
            Times.Once);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
