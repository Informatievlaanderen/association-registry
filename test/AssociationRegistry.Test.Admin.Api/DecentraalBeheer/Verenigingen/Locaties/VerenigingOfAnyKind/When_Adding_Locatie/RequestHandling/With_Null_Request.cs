namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Adding_Locatie.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Request
{
    private readonly VoegLocatieToeController _controller;

    public With_Null_Request()
    {
        var messageBus = Mock.Of<IMessageBus>();

        _controller = new VoegLocatieToeController(
            messageBus,
            new VoegLocatieToeValidator(),
            new AppSettings()
            {
                BaseUrl = "https://beheer.verenigingen.vlaanderen.be",
            });
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Post(
                vCode: "V001001",
                null!,
                new CommandMetadataProviderStub { Initiator = "OVO0001000" },
                ifMatch: "M/\"1\""));
    }
}
