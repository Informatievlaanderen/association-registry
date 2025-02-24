namespace AssociationRegistry.Test.Admin.Api.Controllers;

using AssociationRegistry.Admin.Api.Infrastructure.ResponseWriter;
using AssociationRegistry.Admin.Api.Infrastructure.Sequence;
using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Admin.Api.Verenigingen.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Common.AutoFixture;
using Hosts.Configuration.ConfigurationBindings;
using Moq;
using Xunit;

public class DetailVerenigingenControllerTests
{
    [Fact]
    public async Task Uses_GetNamesQuery()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var detail = fixture.Create<BeheerVerenigingDetailDocument>();

        var detailQuery = new Mock<IBeheerVerenigingDetailQuery>();

        detailQuery
           .Setup(x => x.ExecuteAsync(new BeheerVerenigingDetailFilter(detail.VCode), It.IsAny<CancellationToken>()))
           .ReturnsAsync(detail);

        var namenVoorLidmaatschappen = detail.Lidmaatschappen.Select(x => new KeyValuePair<string, string>(x.AndereVereniging, fixture.Create<string>())).ToDictionary();

        var getNamesForVCodesFilter = new GetNamesForVCodesFilter(detail.Lidmaatschappen.Select(x => x.AndereVereniging).ToArray());

        var getNamesQuery = new Mock<IGetNamesForVCodesQuery>();
        getNamesQuery
           .Setup(query => query.ExecuteAsync(It.IsAny<GetNamesForVCodesFilter>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(namenVoorLidmaatschappen);

        var sut = new DetailVerenigingenController(new AppSettings());

        await sut.Detail(
            Mock.Of<ISequenceGuarder>(),
            detailQuery.Object,
            getNamesQuery.Object,
            Mock.Of<IResponseWriter>(),
            detail.VCode,
            null,
            null,
            CancellationToken.None);

        getNamesQuery.Verify(x => x.ExecuteAsync(
                                 It.Is<GetNamesForVCodesFilter>(filter => filter.VCodes.SequenceEqual(getNamesForVCodesFilter.VCodes)),
                                 It.IsAny<CancellationToken>()), Times.Once);
    }

}
