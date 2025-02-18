namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging.Publiek.MutatieDienst;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Public.Api.Verenigingen.Mutaties;
using Xunit;
using Contactgegeven = Public.Api.Verenigingen.Detail.ResponseModels.Contactgegeven;
using DoelgroepResponse = Public.Api.Verenigingen.Detail.ResponseModels.DoelgroepResponse;
using GestructureerdeIdentificator = Public.Api.Verenigingen.Detail.ResponseModels.GestructureerdeIdentificator;
using HoofdactiviteitVerenigingsloket = Public.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = Public.Api.Verenigingen.Detail.ResponseModels.Locatie;
using Relatie = Public.Api.Verenigingen.Detail.ResponseModels.Relatie;
using Sleutel = Public.Api.Verenigingen.Detail.ResponseModels.Sleutel;
using Vereniging = Public.Api.Verenigingen.Detail.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using VerenigingsType = Public.Api.Verenigingen.Detail.ResponseModels.VerenigingsType;
using Werkingsgebied = Public.Api.Verenigingen.Detail.ResponseModels.Werkingsgebied;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VerenigingMutationsSequenceResponse : End2EndTest<RegistreerFeitelijkeVerenigingTestContext, RegistreerFeitelijkeVerenigingRequest, PubliekVerenigingSequenceResponse[]>
{
    private readonly RegistreerFeitelijkeVerenigingTestContext _testContext;

    public Returns_VerenigingMutationsSequenceResponse(RegistreerFeitelijkeVerenigingTestContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_RegisteredVereniging()
    {
        var actual = Response.SingleOrDefault(x => x.VCode == _testContext.VCode);
        actual.Should().NotBeNull();
        actual.Sequence.Should().BeGreaterThan(0);
    }

    public override Func<IApiSetup, PubliekVerenigingSequenceResponse[]> GetResponse
        => setup => setup.PublicApiHost.GetVerenigingMutationsSequence();
}
