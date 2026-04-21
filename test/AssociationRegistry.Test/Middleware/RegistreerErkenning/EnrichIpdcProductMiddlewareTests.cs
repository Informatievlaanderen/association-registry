namespace AssociationRegistry.Test.Middleware.RegistreerErkenning;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning.Middleware;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Ipdc;
using FluentAssertions;
using Integrations.Ipdc.Clients;
using Integrations.Ipdc.Responses;
using Moq;
using Resources;
using Xunit;

public class EnrichIpdcProductMiddlewareTests
{
    private readonly Fixture _fixture;

    public EnrichIpdcProductMiddlewareTests()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async Task With_Empty_Response_From_Ipdc_Then_Throws_IpdcException()
    {
        var commandEnvelope = new CommandEnvelope<RegistreerErkenningCommand>(
            _fixture.Create<RegistreerErkenningCommand>(),
            _fixture.Create<CommandMetadata>()
        );

        var ipdcClientMock = new Mock<IIpdcClient>();

        ipdcClientMock
           .Setup(x => x.GetById(commandEnvelope.Command.Erkenning.IpdcProductNummer, It.IsAny<CancellationToken>()))
           .ReturnsAsync((IpdcProductResponse?)null);

        var exception = await Assert.ThrowsAnyAsync<IpdcException>(async () =>
                                                                       await EnrichIpdcProductMiddleware.BeforeAsync(
                                                                           commandEnvelope,
                                                                           ipdcClientMock.Object)
        );

        exception
           .Message.Should()
           .Be(
                string.Format(
                    ExceptionMessages.IpdcException,
                    string.Format(
                        ExceptionMessages.IpdcLegeResponseException,
                        commandEnvelope.Command.Erkenning.IpdcProductNummer
                    )
                ));
    }

    [Fact]
    public async Task With_Empty_NL_Name_Response_From_Ipdc_Then_Throws_IpdcException()
    {
        var response = _fixture.Create<IpdcProductResponse>() with
        {
            Naam = _fixture.Create<Translation>() with
            {
                Nl = null,
            },
        };

        var commandEnvelope = new CommandEnvelope<RegistreerErkenningCommand>(
            _fixture.Create<RegistreerErkenningCommand>(),
            _fixture.Create<CommandMetadata>()
        );

        var ipdcClientMock = new Mock<IIpdcClient>();

        ipdcClientMock
           .Setup(x => x.GetById(commandEnvelope.Command.Erkenning.IpdcProductNummer, It.IsAny<CancellationToken>()))
           .ReturnsAsync(response);

        var exception = await Assert.ThrowsAnyAsync<IpdcException>(async () =>
                                                                       await EnrichIpdcProductMiddleware.BeforeAsync(
                                                                           commandEnvelope,
                                                                           ipdcClientMock.Object)
        );

        exception
           .Message.Should()
           .Be(
                string.Format(
                    ExceptionMessages.IpdcException,
                    string.Format(
                        ExceptionMessages.IpdcLegeNaamException,
                        commandEnvelope.Command.Erkenning.IpdcProductNummer
                    )
                ));
    }
}
