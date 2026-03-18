namespace AssociationRegistry.Test.E2E.When_Verwijder_Validatie_Bankrekeningnummer.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging.Bronnen;
using Xunit;
using Bankrekeningnummer = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Bankrekeningnummer;

[Collection(nameof(MaakValidatieBankrekeningnummerOngedaanCollection))]
public class Returns_Detail_Without_Validatie_Bankrekeningnummer : End2EndTest<DetailVerenigingResponse>
{
    private readonly MaakValidatieBankrekeningnummerOngedaanContext _testOngedaanContext;

    public Returns_Detail_Without_Validatie_Bankrekeningnummer(MaakValidatieBankrekeningnummerOngedaanContext testOngedaanContext)
        : base(testOngedaanContext.ApiSetup)
    {
        _testOngedaanContext = testOngedaanContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerDetail(
            setup.AdminHttpClient,
            _testOngedaanContext.VCode,
            new RequestParameters().WithExpectedSequence(_testOngedaanContext.CommandResult.Sequence)
        );

    [Fact]
    public void JsonContentMatches()
    {
        var bankrekeningnummersFromRegistreer =
            _testOngedaanContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Bankrekeningnummers.Select(
                x => new Bankrekeningnummer()
                {
                    type = JsonLdType.Bankrekeningnummer.Type,
                    id = JsonLdType.Bankrekeningnummer.CreateWithIdValues(
                        _testOngedaanContext.VCode,
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
                                _testOngedaanContext.VCode,
                                _testOngedaanContext.Scenario.BankrekeningnummerWerdToegevoegdVoorValidatie.BankrekeningnummerId.ToString()
                            ),
                            BankrekeningnummerId = _testOngedaanContext
                                .Scenario
                                .BankrekeningnummerWerdToegevoegdVoorValidatie
                                .BankrekeningnummerId,
                            Iban = _testOngedaanContext.Scenario.BankrekeningnummerWerdToegevoegdVoorValidatie.Iban,
                            Doel = _testOngedaanContext.Scenario.BankrekeningnummerWerdToegevoegdVoorValidatie.Doel,
                            Titularis = _testOngedaanContext.Scenario.BankrekeningnummerWerdToegevoegdVoorValidatie.Titularis,
                            BevestigdDoor = [],
                            Bron = Bron.Initiator,
                        }
                    )
                    .OrderBy(x => x.BankrekeningnummerId)
                    .ToArray()
            );
    }
}
