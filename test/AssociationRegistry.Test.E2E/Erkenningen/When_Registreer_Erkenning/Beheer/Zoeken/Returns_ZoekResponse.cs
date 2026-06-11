namespace AssociationRegistry.Test.E2E.Erkenningen.When_Registreer_Erkenning.Beheer.Zoeken;

using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten;
using Xunit;

[Collection(nameof(RegistreerErkenningCollection))]
public class Returns_ZoekResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly RegistreerErkenningContext _testContext;

    public Returns_ZoekResponse(RegistreerErkenningContext testContext)
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
                        _testContext.CommandRequest.Erkenning.Startdatum,
                        _testContext.CommandRequest.Erkenning.Einddatum
                    ),
                    DateOnly.FromDateTime(DateTime.Now)
                )
                .Value == ErkenningStatus.Actief.Value;

        Response.Verenigingen.Single().IsErkend.Should().Be(isErkend);
    }
}
