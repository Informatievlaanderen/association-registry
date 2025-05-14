namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging.Beheer.Zoeken.Without_Header;

using Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid;
using Xunit;
using Vereniging = Admin.Api.Verenigingen.Search.ResponseModels.Vereniging;

[Collection(nameof(VerfijnSubtypeNaarFeitelijkeVerenigingCollection))]
public class Returns_Detail : End2EndTest<SearchVerenigingenResponse>
{
    private readonly VerfijnSubtypeNaarFeitelijkeVerenigingContext _testContext;

    public Returns_Detail(VerfijnSubtypeNaarFeitelijkeVerenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient, $"vCode:{_testContext.VCode}",new RequestParameters()
                                                 .WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();
    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
    {
        var vereniging = Response.Verenigingen.Single();
        vereniging.VCode.Should().BeEquivalentTo(_testContext.VCode);
        vereniging.Verenigingssubtype.Should().BeNull();
    }
}
