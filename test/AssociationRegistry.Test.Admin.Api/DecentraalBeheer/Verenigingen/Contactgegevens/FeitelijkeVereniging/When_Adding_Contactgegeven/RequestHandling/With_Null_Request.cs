namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Adding_Contactgegeven.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using Moq;
using System.Threading.Tasks;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Request
{
    private readonly VoegContactgegevenToeController _controller;

    public With_Null_Request()
    {
        var messageBus = Mock.Of<IMessageBus>();

        _controller = new VoegContactgegevenToeController(
            messageBus,
            new VoegContactgegevenToeValidator(),
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
                new CommandMetadataProviderStub(),
                ifMatch: "M/\"1\""));
    }
}
