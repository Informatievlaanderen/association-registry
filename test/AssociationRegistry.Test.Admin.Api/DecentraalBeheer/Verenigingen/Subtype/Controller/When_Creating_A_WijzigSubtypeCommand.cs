namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.Controller;

using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Api.Verenigingen.Subtype;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using FluentValidation;
using Hosts.Configuration.ConfigurationBindings;
using Moq;
using Vereniging;
using Wolverine;
using Xunit;

public class When_Creating_A_WijzigSubtypeCommand
{
    // TODO: Write test that checks if we get the name when it's not null
    // todo: use GetNamesQuery instead
    [Fact]
    public async ValueTask Fetches_The_Name_AndereVereniging_From_Projections()
    {
        // var fixture = new Fixture().CustomizeAdminApi();
        // var messageBus = new Mock<IMessageBus>();
        //
        // messageBus
        //    .Setup(x => x.InvokeAsync<EntityCommandResult>(It.IsAny<CommandEnvelope<WijzigSubtypeCommand>>(), default, null))
        //    .ReturnsAsync(new Fixture().CustomizeAdminApi().Create<EntityCommandResult>());
        //
        // var sut = new WijzigSubtypeController(messageBus.Object, new AppSettings());
        //
        // var vCode = fixture.Create<VCode>();
        // var andereVerenigingNaam = fixture.Create<string>();
        //
        // var WijzigSubtypeRequest = new WijzigSubtypeRequest()
        // {
        //     Subtype = Subtype.FeitelijkeVereniging.Code,
        // };
        //
        // var detailQuery = new Mock<IBeheerVerenigingDetailQuery>();
        //
        // detailQuery.Setup(query => query.ExecuteAsync(new BeheerVerenigingDetailFilter(WijzigSubtypeRequest.AndereVereniging),
        //                                               It.IsAny<CancellationToken>()))
        //            .ReturnsAsync(fixture.Create<BeheerVerenigingDetailDocument>() with
        //             {
        //                 Naam = andereVerenigingNaam,
        //             });
        //
        // await sut.WijzigSubtype(vCode, WijzigSubtypeRequest,
        //                         detailQuery.Object,
        //                         Mock.Of<IValidator<WijzigSubtypeRequest>>(),
        //                         Mock.Of<ICommandMetadataProvider>());
        //
        // var toeTeVoegenLidmaatschap = new WijzigSubtypeCommand
        //     .TeWijzigenSubtype(new Subtype(Subtype.FeitelijkeVereniging.Code, Subtype.FeitelijkeVereniging.Naam),
        //                        VCode.Create(WijzigSubtypeRequest.AndereVereniging),
        //                        andereVerenigingNaam,
        //                        SubtypeIdentificatie.Create(WijzigSubtypeRequest.Identificatie),
        //                        SubtypeBeschrijving.Create(WijzigSubtypeRequest.Beschrijving));
        //
        // messageBus.Verify(
        //     bus => bus.InvokeAsync<EntityCommandResult>(
        //         It.Is<CommandEnvelope<WijzigSubtypeCommand>>(
        //             cmd =>
        //                 cmd.Command.SubtypeData.AndereVereniging == toeTeVoegenLidmaatschap.AndereVereniging &&
        //                 cmd.Command.SubtypeData.AndereVerenigingNaam == andereVerenigingNaam &&
        //                 cmd.Command.SubtypeData.Subtype ==
        //                 new Subtype(Subtype.FeitelijkeVereniging.Code, Subtype.FeitelijkeVereniging.Naam) &&
        //                 cmd.Command.SubtypeData.Identificatie == toeTeVoegenLidmaatschap.Identificatie &&
        //                 cmd.Command.SubtypeData.Beschrijving == toeTeVoegenLidmaatschap.Beschrijving),
        //         default, null), Times.Once);
    }
}
