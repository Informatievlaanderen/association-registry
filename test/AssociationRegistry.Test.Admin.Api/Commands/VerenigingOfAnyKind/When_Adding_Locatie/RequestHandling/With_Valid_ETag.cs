namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.RequestHandling;

using Acties.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
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
    private readonly VoegLocatieToeController _controller;
    private readonly Fixture _fixture;
    private const int ETagNumber = 1;

    public With_Valid_ETag()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _messageBusMock = new Mock<IMessageBus>();

        _messageBusMock
           .Setup(x => x.InvokeAsync<EntityCommandResult>(It.IsAny<CommandEnvelope<VoegLocatieToeCommand>>(), default, null))
           .ReturnsAsync(new Fixture().CustomizeAdminApi().Create<EntityCommandResult>());

        _controller = new VoegLocatieToeController(
                _messageBusMock.Object,
                new VoegLocatieToeValidator(),
                new AppSettings()
                {
                    BaseUrl = "https://beheer.verenigingen.vlaanderen.be",
                })
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async Task InitializeAsync()
    {
        await _controller.Post(
            _fixture.Create<VCode>(),
            _fixture.Create<VoegLocatieToeRequest>(),
            _fixture.Create<CommandMetadataProviderStub>(),
            $"W/\"{ETagNumber}\"");
    }

    [Fact]
    public void Then_it_invokes_with_a_correct_version_number()
    {
        _messageBusMock.Verify(
            expression: messageBus =>
                messageBus.InvokeAsync<EntityCommandResult>(
                    It.Is<CommandEnvelope<VoegLocatieToeCommand>>(
                        env =>
                            env.Metadata.ExpectedVersion == ETagNumber),
                    default,
                    null),
            Times.Once);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
