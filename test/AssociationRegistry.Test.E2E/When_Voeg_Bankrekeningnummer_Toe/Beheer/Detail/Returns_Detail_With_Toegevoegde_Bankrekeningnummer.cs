namespace AssociationRegistry.Test.E2E.When_Voeg_Bankrekeningnummer_Toe.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Be.Vlaanderen.Basisregisters.Utilities;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging.Bankrekeningen;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging.Bronnen;
using Xunit;
using Bankrekeningnummer = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Bankrekeningnummer;

[Collection(nameof(VoegBankrekeningnummerToeCollection))]
public class Returns_Detail_With_Toegevoegde_Bankrekeningnummer : End2EndTest<DetailVerenigingResponse>
{
    private readonly VoegBankrekeningnummerToeContext _testContext;

    public Returns_Detail_With_Toegevoegde_Bankrekeningnummer(VoegBankrekeningnummerToeContext testContext)
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

        var nextId =
            _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Bankrekeningnummers.Max(
                x => x.BankrekeningnummerId
            ) + 1;

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
                                nextId.ToString()
                            ),
                            BankrekeningnummerId = nextId,
                            Iban = _testContext.CommandRequest.Bankrekeningnummer.Iban,
                            Doel = _testContext.CommandRequest.Bankrekeningnummer.Doel,
                            Titularis = _testContext.CommandRequest.Bankrekeningnummer.Titularis,
                            Bron = Bron.Initiator,
                        }
                    )
                    .OrderBy(x => x.BankrekeningnummerId)
                    .ToArray()
            );
    }
}
