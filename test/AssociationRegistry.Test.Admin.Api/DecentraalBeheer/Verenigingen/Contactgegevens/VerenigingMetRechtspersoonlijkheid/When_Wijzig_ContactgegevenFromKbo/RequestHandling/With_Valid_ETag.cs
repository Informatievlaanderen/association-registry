namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.When_Wijzig_ContactgegevenFromKbo.RequestHandling;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven.RequestModels;
using AssociationRegistry.DecentraalBeheer.Contactgegevens.WijzigContactgegevenFromKbo;
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
using Xunit.Categories;

[UnitTest]
public class With_Valid_ETag : IAsyncLifetime
{
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly WijzigContactgegevenController _controller;
    private const int ETagNumber = 1;

    public With_Valid_ETag()
    {
        _messageBusMock = new Mock<IMessageBus>();

        _messageBusMock
           .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<WijzigContactgegevenFromKboCommand>>(), default, null))
           .ReturnsAsync(new Fixture().CustomizeAdminApi().Create<CommandResult>());

        _controller = new WijzigContactgegevenController(_messageBusMock.Object, new WijzigContactgegevenValidator(), new AppSettings())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async ValueTask InitializeAsync()
    {
        await _controller.Patch(
            vCode: "V0001001",
            contactgegevenId: 1,
            new WijzigContactgegevenRequest
            {
                Contactgegeven = new TeWijzigenContactgegeven
                {
                    Beschrijving = "Beschrijving",
                },
            },
            new CommandMetadataProviderStub { Initiator = "OVO0001000" },
            $"W/\"{ETagNumber}\"");
    }

    [Fact]
    public void Then_it_invokes_with_a_correct_version_number()
    {
        _messageBusMock.Verify(
            expression: messageBus =>
                messageBus.InvokeAsync<CommandResult>(
                    It.Is<CommandEnvelope<WijzigContactgegevenFromKboCommand>>(
                        env =>
                            env.Metadata.ExpectedVersion == ETagNumber),
                    default,
                    null),
            Times.Once);
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}
