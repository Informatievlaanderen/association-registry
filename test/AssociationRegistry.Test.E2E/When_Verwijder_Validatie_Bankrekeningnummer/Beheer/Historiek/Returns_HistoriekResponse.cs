namespace AssociationRegistry.Test.E2E.When_Verwijder_Validatie_Bankrekeningnummer.Beheer.Historiek;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(MaakValidatieBankrekeningnummerOngedaanCollection))]
public class Returns_Historiek_Met_Bankrekeningnummer : End2EndTest<HistoriekResponse>
{
    private readonly MaakValidatieBankrekeningnummerOngedaanContext _testOngedaanContext;

    public Returns_Historiek_Met_Bankrekeningnummer(MaakValidatieBankrekeningnummerOngedaanContext testOngedaanContext) : base(testOngedaanContext.ApiSetup)
    {
        _testOngedaanContext = testOngedaanContext;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient, _testOngedaanContext.VCode, headers: new RequestParameters().WithExpectedSequence(_testOngedaanContext.CommandResult.Sequence));

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_BankrekeningnummerWerdToegevoegd_Gebeurtenissen()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd));
        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.BankrekeningnummerWerdGevalideerd(_testOngedaanContext.Scenario.BankrekeningnummerWerdToegevoegdVoorValidatie),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
