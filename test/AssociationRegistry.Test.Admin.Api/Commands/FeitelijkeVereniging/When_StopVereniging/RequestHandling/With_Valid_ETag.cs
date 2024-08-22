namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_StopVereniging.RequestHandling;

using Acties.StopVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.Stop;
using AssociationRegistry.Admin.Api.Verenigingen.Stop.RequestModels;
using AssociationRegistry.Framework;
using AutoFixture;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Vereniging;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_ETag : IAsyncLifetime
{
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly StopVerenigingController _controller;
    private const int ETagNumber = 1;

    public With_Valid_ETag()
    {
        _messageBusMock = new Mock<IMessageBus>();

        _messageBusMock
           .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<StopVerenigingCommand>>(), default, null))
           .ReturnsAsync(new Fixture().CustomizeAdminApi().Create<CommandResult>());

        _controller = new StopVerenigingController(_messageBusMock.Object, new AppSettings(), new StopVerenigingRequestValidator())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async Task InitializeAsync()
    {
        await _controller.Post(
            new StopVerenigingRequest
                { Einddatum = new DateOnly() },
            vCode: "V0001001",
            new CommandMetadataProviderStub { Initiator = "OVO0001000" },
            $"W/\"{ETagNumber}\"");
    }

    [Fact]
    public void Then_it_invokes_with_a_correct_version_number()
    {
        _messageBusMock.Verify(
            expression: messageBus =>
                messageBus.InvokeAsync<CommandResult>(
                    It.Is<CommandEnvelope<StopVerenigingCommand>>(
                        env =>
                            env.Metadata.ExpectedVersion == ETagNumber),
                    default,
                    null),
            Times.Once);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
