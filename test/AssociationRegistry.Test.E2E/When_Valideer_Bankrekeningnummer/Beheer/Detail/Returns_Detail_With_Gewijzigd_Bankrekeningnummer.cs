namespace AssociationRegistry.Test.E2E.When_Valideer_Bankrekeningnummer.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using DecentraalBeheer.Vereniging.Bankrekeningen;
using FluentAssertions;
using Xunit;
using Bankrekeningnummer = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Bankrekeningnummer;

[Collection(nameof(ValideerBankrekeningnummerCollection))]
public class Returns_Detail_With_GeValideerd_Bankrekeningnummer : End2EndTest<DetailVerenigingResponse>
{
    private readonly ValideerBankrekeningnummerContext _testContext;

    public Returns_Detail_With_GeValideerd_Bankrekeningnummer(ValideerBankrekeningnummerContext testContext)
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
                    Doel = _testContext.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                    Titularis = _testContext.Scenario.BankrekeningnummerWerdToegevoegd.Titularis,
                    IsGevalideerd = true,
                    Bron = BankrekeningnummerBron.Gi.Value,
                },
            ]);
    }
}
