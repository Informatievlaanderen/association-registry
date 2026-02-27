namespace AssociationRegistry.Test.E2E.When_Valideer_Bankrekeningnummer.Beheer.Detail;

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
        var bankrekeningnummersFromRegistreer =
            _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Bankrekeningnummers.Select(
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
            );

        Response
            .Vereniging.Bankrekeningnummers.Should()
            .BeEquivalentTo(
                bankrekeningnummersFromRegistreer
                    .Append(
                        new Bankrekeningnummer()
                        {
                            type = JsonLdType.Bankrekeningnummer.Type,
                            id = JsonLdType.Bankrekeningnummer.CreateWithIdValues(
                                _testContext.VCode,
                                _testContext.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId.ToString()
                            ),
                            BankrekeningnummerId = _testContext
                                .Scenario
                                .BankrekeningnummerWerdToegevoegd
                                .BankrekeningnummerId,
                            Iban = _testContext.Scenario.BankrekeningnummerWerdToegevoegd.Iban,
                            Doel = _testContext.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                            Titularis = _testContext.Scenario.BankrekeningnummerWerdToegevoegd.Titularis,
                            BevestigdDoor = [new Gegevensinitiator(AuthenticationSetup.Initiator)],
                            Bron = Bron.Initiator,
                        }
                    )
                    .OrderBy(x => x.BankrekeningnummerId)
                    .ToArray()
            );
    }
}
