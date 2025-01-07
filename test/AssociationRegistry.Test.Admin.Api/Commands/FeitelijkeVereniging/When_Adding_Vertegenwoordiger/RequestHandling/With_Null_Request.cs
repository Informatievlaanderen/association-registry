namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.RequestHandling;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.VoegVertegenwoordigerToe;
using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Request
{
    private readonly VoegVertegenwoordigerToeController _controller;

    public With_Null_Request()
    {
        var messageBus = Mock.Of<IMessageBus>();
        _controller = new VoegVertegenwoordigerToeController(
            messageBus,
            new VoegVertegenwoordigerToeValidator(),
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
                new CommandMetadataProviderStub { Initiator = "OVO000001" },
                ifMatch: "M/\"1\""));
    }
}
