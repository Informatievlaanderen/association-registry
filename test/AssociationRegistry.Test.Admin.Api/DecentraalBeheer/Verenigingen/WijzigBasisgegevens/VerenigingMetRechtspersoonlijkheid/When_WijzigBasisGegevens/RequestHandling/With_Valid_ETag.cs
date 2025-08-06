namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestHandling;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.DecentraalBeheer.Basisgegevens.VerenigingMetRechtspersoonlijkheid;
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
    private readonly WijzigBasisgegevensController _controller;
    private const int ETagNumber = 1;

    public With_Valid_ETag()
    {
        _messageBusMock = new Mock<IMessageBus>();

        _messageBusMock
           .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<WijzigBasisgegevensCommand>>(), default, null))
           .ReturnsAsync(new Fixture().CustomizeAdminApi().Create<CommandResult>());

        _controller = new WijzigBasisgegevensController(_messageBusMock.Object, new AppSettings())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async ValueTask InitializeAsync()
    {
        await _controller.Patch(
            new WijzigBasisgegevensRequestValidator(),
            new WijzigBasisgegevensRequest { KorteBeschrijving = "Korte naam" },
            vCode: "V0001001",
            new CommandMetadataProviderStub { Initiator = "OVO0001000" },
            Mock.Of<IWerkingsgebiedenService>(),
            $"W/\"{ETagNumber}\"");
    }

    [Fact]
    public void Then_it_invokes_with_a_correct_version_number()
    {
        _messageBusMock.Verify(
            expression: messageBus =>
                messageBus.InvokeAsync<CommandResult>(
                    It.Is<CommandEnvelope<WijzigBasisgegevensCommand>>(
                        env =>
                            env.Metadata.ExpectedVersion == ETagNumber),
                    default,
                    null),
            Times.Once);
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}
