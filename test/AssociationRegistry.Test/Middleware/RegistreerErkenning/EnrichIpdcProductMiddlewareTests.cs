namespace AssociationRegistry.Test.Middleware.RegistreerErkenning;

using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Exceptions;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning.Middleware;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using FluentAssertions;
using Integrations.Ipdc.Clients;
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
    public async Task With_Onbekend_IpdcProduct_ThenThrows_OnbekendIpdcProductNummer()
    {
        var commandEnvelope = new CommandEnvelope<RegistreerErkenningCommand>(
            _fixture.Create<RegistreerErkenningCommand>(),
            _fixture.Create<CommandMetadata>()
        );

        var ipdcClientMock = new Mock<IIpdcClient>();

        ipdcClientMock
            .Setup(x => x.GetById(commandEnvelope.Command.Erkenning.IpdcProductNummer, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OnbekendIpdcProductNummer(commandEnvelope.Command.Erkenning.IpdcProductNummer));

        var exception = await Assert.ThrowsAnyAsync<AdressenregisterReturnedInactiefAdres>(async () =>
            await EnrichIpdcProductMiddleware.BeforeAsync(commandEnvelope, ipdcClientMock.Object)
        );

        exception.Message.Should().Be(ExceptionMessages.OnbekendIpdcProductNummer);
    }
}
