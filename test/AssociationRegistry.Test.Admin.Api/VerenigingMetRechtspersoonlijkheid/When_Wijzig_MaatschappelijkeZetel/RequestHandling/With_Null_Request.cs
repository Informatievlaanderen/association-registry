namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;
using Fakes;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Request
{
    private readonly WijzigMaatschappelijkeZetelController _controller;

    public With_Null_Request()
    {
        var messageBusMock = new MessageBusMock();

        _controller = new WijzigMaatschappelijkeZetelController(messageBusMock, new WijzigMaatschappelijkeZetelRequestValidator(),
                                                                new AppSettings());
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Patch(
                vCode: "V0001001",
                locatieId: 1,
                request: null,
                new CommandMetadataProviderStub { Initiator = "OVO000001" },
                ifMatch: "M/\"1\""));
    }
}
