namespace AssociationRegistry.Test.E2E.V2.When_Stop_Vereniging.Publiek_Detail_All;

using Alba;
using AssociationRegistry.Admin.Api.Verenigingen.Stop.RequestModels;
using AssociationRegistry.Test.E2E.Data;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using E2E.When_Stop_Vereniging;
using FluentAssertions;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(RegistreerVerenigingAdminCollection2.Name)]
public class Returns_ArrayOfDetailResponses : IClassFixture<StopVerenigingContext>, IAsyncLifetime
{
    private readonly StopVerenigingContext _context;

    public Returns_ArrayOfDetailResponses(StopVerenigingContext context)
    {
        _context = context;
    }

    [Fact]
    public void WithVereniging()
        => Response.Single().Should().BeEquivalentTo(new TeVerwijderenVereniging
        {
            Vereniging = new TeVerwijderenVerenigingData()
            {
                VCode = _context.VCode,
                TeVerwijderen = true,
            },
        });

    public async Task InitializeAsync()
    {
        Response = _context.ApiSetup.PublicApiHost.GetPubliekDetailAll<PubliekVerenigingDetailResponse>();
    }

    public PubliekVerenigingDetailResponse[] Response { get; set; }

    public async Task DisposeAsync()
    {
    }
}
