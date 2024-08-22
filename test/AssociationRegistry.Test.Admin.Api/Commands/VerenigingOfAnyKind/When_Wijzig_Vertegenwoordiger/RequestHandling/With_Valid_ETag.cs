namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.RequestHandling;

using Acties.WijzigVertegenwoordiger;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger.RequestModels;
using AssociationRegistry.Framework;
using AutoFixture;
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
    private readonly WijzigVertegenwoordigerController _toeController;
    private readonly Fixture _fixture;
    private const int ETagNumber = 1;

    public With_Valid_ETag()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _messageBusMock = new Mock<IMessageBus>();

        _messageBusMock
           .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<object>(), default, null))
           .ReturnsAsync(new Fixture().CustomizeAdminApi().Create<CommandResult>());

        _toeController = new WijzigVertegenwoordigerController(_messageBusMock.Object, new ValidatorStub<WijzigVertegenwoordigerRequest>())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async Task InitializeAsync()
    {
        await _toeController.Patch(
            _fixture.Create<VCode>(),
            _fixture.Create<int>(),
            _fixture.Create<WijzigVertegenwoordigerRequest>(),
            _fixture.Create<CommandMetadataProviderStub>(),
            $"W/\"{ETagNumber}\"");
    }

    [Fact]
    public void Then_it_invokes_with_a_correct_version_number()
    {
        _messageBusMock.Verify(
            expression: messageBus =>
                messageBus.InvokeAsync<CommandResult>(
                    It.Is<CommandEnvelope<WijzigVertegenwoordigerCommand>>(
                        env =>
                            env.Metadata.ExpectedVersion == ETagNumber),
                    default,
                    null),
            Times.Once);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
