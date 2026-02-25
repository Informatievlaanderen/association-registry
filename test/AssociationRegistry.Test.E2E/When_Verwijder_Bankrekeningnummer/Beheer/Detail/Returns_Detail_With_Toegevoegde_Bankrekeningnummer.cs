namespace AssociationRegistry.Test.E2E.When_Verwijder_Bankrekeningnummer.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using DecentraalBeheer.Vereniging.Bankrekeningen;
using FluentAssertions;
using Vereniging.Bronnen;
using Xunit;
using Bankrekeningnummer = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Bankrekeningnummer;

[Collection(nameof(VerwijderBankrekeningnummerCollection))]
public class Returns_Detail_Without_Verwijderd_Bankrekeningnummer : End2EndTest<DetailVerenigingResponse>
{
    private readonly VerwijderBankrekeningnummerContext _testContext;

    public Returns_Detail_Without_Verwijderd_Bankrekeningnummer(VerwijderBankrekeningnummerContext testContext)
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
        var bankrekeningnummersFromRegistreer = _testContext
            .Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Bankrekeningnummers.Select(
                x => new Bankrekeningnummer()
                {
                    type = JsonLdType.Bankrekeningnummer.Type,
                    id = JsonLdType.Bankrekeningnummer.CreateWithIdValues(
                        _testContext.VCode,
                        x.BankrekeningnummerId.ToString()
                    ),
                    BankrekeningnummerId = x.BankrekeningnummerId,
                    Iban = x.Iban,
                    Doel = x.Doel,
                    Titularis = x.Titularis,
                    BevestigdDoor = [],
                    Bron = Bron.Initiator,
                }
            )
            .OrderBy(x => x.BankrekeningnummerId);

        // scenario.Toegevoegd one is deleted so only check if the bankreknrs from registreer are expected
        Response.Vereniging.Bankrekeningnummers.Should().BeEquivalentTo(bankrekeningnummersFromRegistreer);
    }
}
