﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Removing_Contactgegeven.RequestHandling;

using Acties.VerwijderContactgegeven;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VerwijderContactgegeven;
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
    private readonly VerwijderContactgegevenController _toeController;
    private readonly Fixture _fixture;
    private const int ETagNumber = 1;
    private readonly CommandMetadataProviderStub _initiatorProvider = new() { Initiator = "OVO000001" };

    public With_Valid_ETag()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _messageBusMock = new Mock<IMessageBus>();
        _messageBusMock
            .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<VerwijderContactgegevenCommand>>(), default, null))
            .ReturnsAsync(new Fixture().CustomizeAdminApi().Create<CommandResult>());

        _toeController = new VerwijderContactgegevenController(_messageBusMock.Object)
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async Task InitializeAsync()
    {
        await _toeController.Delete(
            _fixture.Create<VCode>(),
            _fixture.Create<int>(),
            _initiatorProvider,
            $"W/\"{ETagNumber}\"");
    }

    [Fact]
    public void Then_it_invokes_with_a_correct_version_number()
    {
        _messageBusMock.Verify(
            messageBus =>
                messageBus.InvokeAsync<CommandResult>(
                    It.Is<CommandEnvelope<VerwijderContactgegevenCommand>>(
                        env =>
                            env.Metadata.ExpectedVersion == ETagNumber),
                    default,
                    null),
            Times.Once);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
