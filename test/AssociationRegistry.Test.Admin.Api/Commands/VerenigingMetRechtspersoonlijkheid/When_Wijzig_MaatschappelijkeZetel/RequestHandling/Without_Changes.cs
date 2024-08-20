namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.RequestHandling;

using AssociationRegistry.Acties.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Without_Changes : IAsyncLifetime
{
    private readonly WijzigMaatschappelijkeZetelController _controller;
    private IActionResult _result = null!;

    public Without_Changes()
    {
        var messageBusMock = new Mock<IMessageBus>();

        messageBusMock
           .Setup(x => x.InvokeAsync<CommandResult>(It.IsAny<CommandEnvelope<WijzigMaatschappelijkeZetelCommand>>(), default, null))
           .ReturnsAsync(CommandResult.Create(VCode.Create("V0001001"), StreamActionResult.Empty));

        _controller = new WijzigMaatschappelijkeZetelController(messageBusMock.Object, new WijzigMaatschappelijkeZetelRequestValidator(),
                                                                new AppSettings())
            { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
    }

    public async Task InitializeAsync()
    {
        _result = await _controller.Patch(
            vCode: "V0001001",
            locatieId: 1,
            new WijzigMaatschappelijkeZetelRequest
            {
                Locatie = new TeWijzigenMaatschappelijkeZetel
                {
                    Naam = "naam",
                },
            },
            new CommandMetadataProviderStub { Initiator = "OVO000001" },
            ifMatch: "W/\"1\"");
    }

    [Fact]
    public void Then_it_returns_an_ok_response()
    {
        _result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public void Then_it_returns_no_sequence_header()
    {
        _controller.Response.Headers.Should().NotContainKey(WellknownHeaderNames.Sequence);
    }

    [Fact]
    public void Then_it_returns_no_location_header()
    {
        _controller.Response.Headers.Should().NotContainKey(HeaderNames.Location);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
