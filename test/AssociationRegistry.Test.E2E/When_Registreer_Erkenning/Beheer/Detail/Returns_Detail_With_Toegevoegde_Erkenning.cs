namespace AssociationRegistry.Test.E2E.When_Registreer_Erkenning.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(RegistreerErkenningCollection))]
public class Returns_Detail_With_Toegevoegde_Erkenning : End2EndTest<DetailVerenigingResponse>
{
    private readonly RegistreerErkenningContext _testContext;

    public Returns_Detail_With_Toegevoegde_Erkenning(RegistreerErkenningContext testContext)
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
        var erkenningFromRegistreer =
            _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Erkenningen.Select(
                x => new Erkenning
                {
                    id = JsonLdType.Erkenning.CreateWithIdValues(
                        _testContext.VCode,
                        x.ErkenningId.ToString()
                    ),
                    type = JsonLdType.Erkenning.Type,
                    ErkenningId = 0,
                    VCode = _testContext.VCode.Value,
                    GeregistreerdDoor = [],
                    IpdcProduct = new IpdcProduct
                    {
                        Nummer = x.IpdcProductNummer,
                    },
                    Startdatum = x.Startdatum,
                    Einddatum = x.Startdatum,
                    Hernieuwingsdatum = x.Hernieuwingsdatum,
                    HernieuwingsUrl = x.HernieuwingsUrl,

                }
            );

        var nextId =
            _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Erkenningen.Max(
                x => x.ErkenningId
            ) + 1;

        Response
            .Vereniging.Erkenningen.Should()
            .BeEquivalentTo(
                erkenningFromRegistreer
                    .Append(
                        new Erkenning
                        {
                            type = JsonLdType.Erkenning.Type,
                            id = JsonLdType.Erkenning.CreateWithIdValues(
                                _testContext.VCode,
                                nextId.ToString()
                            ),
                            ErkenningId = nextId,
                            VCode = _testContext.VCode.Value,
                            GeregistreerdDoor = [],
                            IpdcProduct = new IpdcProduct
                            {
                                Nummer = _testContext.CommandRequest.Erkenning.IpdcProductNummer,
                            },
                            Startdatum = _testContext.CommandRequest.Erkenning.Startdatum,
                            Einddatum = _testContext.CommandRequest.Erkenning.Einddatum,
                            Hernieuwingsdatum = _testContext.CommandRequest.Erkenning.Hernieuwingsdatum,
                            HernieuwingsUrl = _testContext.CommandRequest.Erkenning.HernieuwingsUrl,

                        }
                    )
                    .OrderBy(x => x.ErkenningId)
                    .ToArray()
            );
    }
}
