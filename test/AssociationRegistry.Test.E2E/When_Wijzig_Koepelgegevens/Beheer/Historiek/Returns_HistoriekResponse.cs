namespace AssociationRegistry.Test.E2E.When_Wijzig_Koepelgegevens.Beheer.Historiek;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(WijzigKoepelgegevensCollection))]
public class Returns_HistoriekResponse : End2EndTest<HistoriekResponse>
{
    private readonly WijzigKoepelgegevensContext _testContext;

    public Returns_HistoriekResponse(WijzigKoepelgegevensContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerHistoriek(
            setup.AdminHttpClient,
            _testContext.CommandResult.VCode,
            headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(_testContext.VCode);
    }

    [Fact]
    public void With_VerenigingWerdGeidentificeerdAlsKoepel_Gebeurtenis()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(VerenigingWerdGeidentificeerdAlsKoepel)
        );

        gebeurtenisResponse.ShouldCompare(
            HistoriekGebeurtenisMapper.VerenigingWerdGeidentificeerdAlsKoepel(_testContext.VCode.Value),
            compareConfig: HistoriekComparisonConfig.Instance
        );
    }

    [Fact]
    public void With_KoepelPrivacyverklaringWerdGeaccepteerd_Gebeurtenis()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(KoepelPrivacyverklaringWerdGeaccepteerd)
        );

        gebeurtenisResponse.ShouldCompare(
            HistoriekGebeurtenisMapper.KoepelPrivacyverklaringWerdGeaccepteerd(_testContext.VCode.Value),
            compareConfig: HistoriekComparisonConfig.Instance
        );
    }
}
