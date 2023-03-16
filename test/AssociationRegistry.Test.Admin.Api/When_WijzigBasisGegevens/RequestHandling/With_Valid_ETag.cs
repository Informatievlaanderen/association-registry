﻿namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using Framework;
using Vereniging;
using Vereniging.WijzigBasisgegevens;
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
    private readonly WijzigBasisgegevensController _controller;
    private const int ETagNumber = 1;

    public With_Valid_ETag()
    {
        _messageBusMock = new Mock<IMessageBus>();
        _messageBusMock
            .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<WijzigBasisgegevensCommand>>(), default, null))
            .ReturnsAsync(new Fixture().CustomizeAll().Create<CommandResult>());

        _controller = new WijzigBasisgegevensController(_messageBusMock.Object, new AppSettings())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async Task InitializeAsync()
    {
        await _controller.Patch(
            new WijzigBasisgegevensRequestValidator(),
            new WijzigBasisgegevensRequest { Initiator = "V0001001", KorteNaam = "Korte naam" },
            "V0001001",
            $"W/\"{ETagNumber}\"");
    }

    [Fact]
    public void Then_it_invokes_with_a_correct_version_number()
    {
        _messageBusMock.Verify(
            messageBus =>
                messageBus.InvokeAsync<CommandResult>(
                    It.Is<CommandEnvelope<WijzigBasisgegevensCommand>>(
                        env =>
                            env.Metadata.ExpectedVersion == ETagNumber),
                    default,
                    null),
            Times.Once);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
