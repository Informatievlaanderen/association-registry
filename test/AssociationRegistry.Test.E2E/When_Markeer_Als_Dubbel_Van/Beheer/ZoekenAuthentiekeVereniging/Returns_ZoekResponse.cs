namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.ZoekenAuthentiekeVereniging;

using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Xunit;

[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<SearchVerenigingenResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;
    private readonly ITestOutputHelper _helper;

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext, ITestOutputHelper helper)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _helper = helper;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerZoeken(
            setup.AdminHttpClient,
            $"vCode:{_testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode}",
            setup.AdminApiHost.DocumentStore(),
            headers: new RequestParameters().WithExpectedSequence(
                _testContext.DubbeleVerenigingWerdSamengevoegd!.Sequence
            ),
            testOutputHelper: _helper
        );

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    private Vereniging AuthentiekeVereniging =>
        Response.Verenigingen.Single(x =>
            x.VCode == _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode
        );

    [Fact]
    public void With_The_Own_KorteNaam_Kept() =>
        AuthentiekeVereniging
            .KorteNaam.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensAuthentiekeVereniging.KorteNaam);

    [Fact]
    public void With_The_Startdatum_Of_The_Dubbele_Vereniging() =>
        AuthentiekeVereniging
            .Startdatum.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.Startdatum!.Value.FormatAsBelgianDate());

    [Fact]
    public void With_The_Doelgroep_Of_The_Dubbele_Vereniging()
    {
        var doelgroep = MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.Doelgroep;

        AuthentiekeVereniging.Doelgroep.Minimumleeftijd.Should().Be(doelgroep.Minimumleeftijd);
        AuthentiekeVereniging.Doelgroep.Maximumleeftijd.Should().Be(doelgroep.Maximumleeftijd);
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
    {
        Response
            .Verenigingen.Single(x =>
                x.VCode == _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode
            )
            .CorresponderendeVCodes.Should()
            .Contain(_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);
    }
}
