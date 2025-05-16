namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Publiek.Detail_All;

using Admin.Api.Verenigingen.Stop.RequestModels;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using FluentAssertions;
using Formats;
using Newtonsoft.Json.Linq;
using NodaTime.Extensions;
using Public.Api.Verenigingen.DetailAll;
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
                            DeletedAt = DateTime.UtcNow.Date.ToInstant().FormatAsBelgianDate(),
                        },
                    });
}
