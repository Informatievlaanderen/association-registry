namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Publiek.Detail_All;

using AssociationRegistry.Admin.Api.Verenigingen.Stop.RequestModels;
using AssociationRegistry.Public.Api.Verenigingen.Detail;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NodaTime.Extensions;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_ArrayOfDetailResponses : End2EndTest<StopVerenigingContext, StopVerenigingRequest, IEnumerable<JObject>>
{
    public override Func<IApiSetup, IEnumerable<JObject>> GetResponse =>
        setup => setup.PublicApiHost.GetPubliekDetailAll();

    public Returns_ArrayOfDetailResponses(StopVerenigingContext context) : base(context)
    {
    }

    [Fact]
    public void WithVereniging()
        => Response.OnlyTeVerwijderen()
                   .Should()
                   .ContainEquivalentOf(new DetailAllConverter.TeVerwijderenVereniging()
                    {
                        Vereniging = new DetailAllConverter.TeVerwijderenVereniging.TeVerwijderenVerenigingData()
                        {
                            VCode = TestContext.VCode,
                            TeVerwijderen = true,
                            DeletedAt = DateTime.UtcNow.Date.ToInstant().ToBelgianDate(),
                        },
                    });
}
