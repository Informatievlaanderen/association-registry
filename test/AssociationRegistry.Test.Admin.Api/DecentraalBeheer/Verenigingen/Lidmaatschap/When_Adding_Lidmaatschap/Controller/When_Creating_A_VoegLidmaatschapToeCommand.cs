﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.When_Adding_Lidmaatschap.Controller;

using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using AssociationRegistry.Admin.Api.Infrastructure.ResponseWriter;
using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe;
using AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.DecentraalBeheer.Lidmaatschappen.VoegLidmaatschapToe;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentValidation;
using Moq;
using Wolverine;
using Xunit;

public class When_Creating_A_VoegLidmaatschapToeCommand
{
    // todo: use GetNamesQuery instead
    [Fact]
    public async ValueTask Fetches_The_Name_AndereVereniging_From_Projections()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var messageBus = new Mock<IMessageBus>();

        messageBus
           .Setup(x => x.InvokeAsync<EntityCommandResult>(It.IsAny<CommandEnvelope<VoegLidmaatschapToeCommand>>(), default, null))
           .ReturnsAsync(new Fixture().CustomizeAdminApi().Create<EntityCommandResult>());

        var sut = new VoegLidmaatschapToeController(messageBus.Object, new AppSettings());

        var vCode = fixture.Create<VCode>();
        var andereVerenigingNaam = fixture.Create<string>();
        var voegLidmaatschapToeRequest = fixture.Create<VoegLidmaatschapToeRequest>();

        var detailQuery = new Mock<IBeheerVerenigingDetailQuery>();

        detailQuery.Setup(query => query.ExecuteAsync(new BeheerVerenigingDetailFilter(voegLidmaatschapToeRequest.AndereVereniging),
                                                      It.IsAny<CancellationToken>()))
                   .ReturnsAsync(fixture.Create<BeheerVerenigingDetailDocument>() with
                    {
                        Naam = andereVerenigingNaam,
                    });

        await sut.VoegLidmaatschapToe(vCode, voegLidmaatschapToeRequest,
                                      Mock.Of<IValidator<VoegLidmaatschapToeRequest>>(),
                                      Mock.Of<ICommandMetadataProvider>(),
                                      detailQuery.Object,
                                      Mock.Of<IResponseWriter>());

        var toeTeVoegenLidmaatschap = new VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap(
            VCode.Create(voegLidmaatschapToeRequest.AndereVereniging),
            andereVerenigingNaam,
            new Geldigheidsperiode(new GeldigVan(voegLidmaatschapToeRequest.Van), new GeldigTot(voegLidmaatschapToeRequest.Tot)),
            LidmaatschapIdentificatie.Create(voegLidmaatschapToeRequest.Identificatie),
            LidmaatschapBeschrijving.Create(voegLidmaatschapToeRequest.Beschrijving));

        messageBus.Verify(
            bus => bus.InvokeAsync<EntityCommandResult>(
                It.Is<CommandEnvelope<VoegLidmaatschapToeCommand>>(
                    cmd =>
                        cmd.Command.Lidmaatschap.AndereVereniging == toeTeVoegenLidmaatschap.AndereVereniging &&
                        cmd.Command.Lidmaatschap.AndereVerenigingNaam == andereVerenigingNaam &&
                        cmd.Command.Lidmaatschap.Identificatie == toeTeVoegenLidmaatschap.Identificatie &&
                        cmd.Command.Lidmaatschap.Beschrijving == toeTeVoegenLidmaatschap.Beschrijving),
                default, null), Times.Once);
    }
}
