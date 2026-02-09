namespace AssociationRegistry.Test.E2E.When_Wijzig_Bankrekeningnummer.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using DecentraalBeheer.Vereniging.Bankrekeningen;
using FluentAssertions;
using Xunit;
using Bankrekeningnummer = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Bankrekeningnummer;

[Collection(nameof(WijzigBankrekeningnummerCollection))]
public class Returns_Detail_With_Gewijzigd_Bankrekeningnummer : End2EndTest<DetailVerenigingResponse>
{
    private readonly WijzigBankrekeningnummerContext _testContext;

    public Returns_Detail_With_Gewijzigd_Bankrekeningnummer(WijzigBankrekeningnummerContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerDetail(
            setup.AdminHttpClient,
            _testContext.VCode,
            new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void JsonContentMatches()
    {
        Response
            .Vereniging.Bankrekeningnummers.Should()
            .BeEquivalentTo([
                new Bankrekeningnummer()
                {
                    type = JsonLdType.Bankrekeningnummer.Type,
                    id = JsonLdType.Bankrekeningnummer.CreateWithIdValues(
                        _testContext.VCode,
                        _testContext.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId.ToString()
                    ),
                    BankrekeningnummerId = _testContext.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                    Iban = _testContext.Scenario.BankrekeningnummerWerdToegevoegd.Iban,
                    Doel = _testContext.CommandRequest.Bankrekeningnummer.Doel,
                    Titularis = _testContext.CommandRequest.Bankrekeningnummer.Titularis,
                    Bron = BankrekeningnummerBron.Gi.Value,
                },
            ]);
    }
}
