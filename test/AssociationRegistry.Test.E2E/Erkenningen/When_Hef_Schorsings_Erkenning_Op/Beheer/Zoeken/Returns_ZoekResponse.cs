namespace AssociationRegistry.Test.E2E.Erkenningen.When_Hef_Schorsings_Erkenning_Op.Beheer.Zoeken;

using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten;
using Xunit;

[Collection(nameof(HefSchorsingErkenningOpCollection))]
public class Returns_ZoekResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly HefSchorsingErkenningOpContext _testContext;

    public Returns_ZoekResponse(HefSchorsingErkenningOpContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerZoeken(
            setup.AdminHttpClient,
            $"vCode:{_testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}",
            setup.AdminApiHost.DocumentStore(),
            headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void IsErkend_Is_Set()
    {
        var isErkend =
            ErkenningStatus
                .Bepaal(
                    ErkenningsPeriode.Create(
                        _testContext.Scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                        _testContext.Scenario.ErkenningWerdGeregistreerd.Einddatum.Value
                    ),
                    DateOnly.FromDateTime(DateTime.Now)
                )
                .Value == ErkenningStatus.Actief.Value;

        Response.Verenigingen.Single().IsErkend.Should().Be(isErkend);
    }
}
