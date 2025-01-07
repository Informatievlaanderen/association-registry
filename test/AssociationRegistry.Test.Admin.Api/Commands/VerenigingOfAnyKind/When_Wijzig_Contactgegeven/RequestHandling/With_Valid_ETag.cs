namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Contactgegeven.RequestHandling;

using Acties.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven.RequestModels;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Framework;
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
    private readonly WijzigContactgegevenController _toeController;
    private readonly Fixture _fixture;
    private const int ETagNumber = 1;

    public With_Valid_ETag()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _messageBusMock = new Mock<IMessageBus>();

        _messageBusMock
           .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<WijzigContactgegevenCommand>>(), default, null))
           .ReturnsAsync(new Fixture().CustomizeAdminApi().Create<CommandResult>());

        _toeController = new WijzigContactgegevenController(_messageBusMock.Object, new WijzigContactgegevenValidator())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async Task InitializeAsync()
    {
        await _toeController.Patch(
            _fixture.Create<VCode>(),
            _fixture.Create<int>(),
            _fixture.Create<WijzigContactgegevenRequest>(),
            _fixture.Create<CommandMetadataProviderStub>(),
            $"W/\"{ETagNumber}\"");
    }

    [Fact]
    public void Then_it_invokes_with_a_correct_version_number()
    {
        _messageBusMock.Verify(
            expression: messageBus =>
                messageBus.InvokeAsync<CommandResult>(
                    It.Is<CommandEnvelope<WijzigContactgegevenCommand>>(
                        env =>
                            env.Metadata.ExpectedVersion == ETagNumber),
                    default,
                    null),
            Times.Once);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
