namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.When_Wijzig_ContactgegevenFromKbo.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Request
{
    private readonly WijzigContactgegevenController _controller;

    public With_Null_Request()
    {
        var messageBusMock = new MessageBusMock();
        _controller = new WijzigContactgegevenController(messageBusMock, new WijzigContactgegevenValidator(), new AppSettings());
    }

    [Fact]
    public async ValueTask Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Patch(
                vCode: "V0001001",
                contactgegevenId: 1,
                request: null,
                new CommandMetadataProviderStub { Initiator = "OVO000001" },
                ifMatch: "M/\"1\""));
    }
}
