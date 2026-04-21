namespace AssociationRegistry.Test.Middleware.RegistreerErkenning;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning.Middleware;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Wegwijs;
using FluentAssertions;
using Integrations.Wegwijs.Clients;
using Integrations.Wegwijs.Responses;
using Moq;
using Resources;
using Xunit;

public class EnrichGegevensInitiatorMiddlewareTests
{
    private readonly Fixture _fixture;

    public EnrichGegevensInitiatorMiddlewareTests()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task With_Null_Or_Empty_Organisation_Name_From_Wegwijs_Then_Throws_WegwijsException(string naam)
    {
        var response = _fixture.Create<OrganisationResponse>() with { Name = naam };

        var commandEnvelope = new CommandEnvelope<RegistreerErkenningCommand>(
            _fixture.Create<RegistreerErkenningCommand>(),
            _fixture.Create<CommandMetadata>()
        );

        var wegwijsClientMock = new Mock<IWegwijsClient>();

        wegwijsClientMock
           .Setup(x => x.GetOrganisationByOvoCode(commandEnvelope.Metadata.Initiator, It.IsAny<CancellationToken>()))
           .ReturnsAsync(response);

        var exception = await Assert.ThrowsAnyAsync<WegwijsException>(async () =>
                                                                          await EnrichGegevensInitiatorMiddleware
                                                                             .BeforeAsync(
                                                                                  commandEnvelope,
                                                                                  wegwijsClientMock.Object)
        );

        exception
           .Message.Should()
           .Be(
                string.Format(
                    ExceptionMessages.WegwijsException,
                    string.Format(
                        ExceptionMessages.WegwijsLegeOrganisatieNaamException,
                        commandEnvelope.Metadata.Initiator
                    )
                ));
    }

    [Fact]
    public async Task With_Valid_Organisation_Response_From_Wegwijs_Then_Returns_GegevensInitiator()
    {
        var response = _fixture.Create<OrganisationResponse>();

        var commandEnvelope = new CommandEnvelope<RegistreerErkenningCommand>(
            _fixture.Create<RegistreerErkenningCommand>(),
            _fixture.Create<CommandMetadata>()
        );

        var wegwijsClientMock = new Mock<IWegwijsClient>();

        wegwijsClientMock
           .Setup(x => x.GetOrganisationByOvoCode(commandEnvelope.Metadata.Initiator, It.IsAny<CancellationToken>()))
           .ReturnsAsync(response);

        var result = await EnrichGegevensInitiatorMiddleware.BeforeAsync(
            commandEnvelope,
            wegwijsClientMock.Object);

        result.Should().BeEquivalentTo(new GegevensInitiator
        {
            OvoCode = commandEnvelope.Metadata.Initiator,
            Naam = response.Name,
        });
    }
}
