namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Publiek.Detail_All;

using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Newtonsoft.Json.Linq;
using NodaTime.Extensions;
using Public.Api.Verenigingen.DetailAll;
using Xunit;

[Collection(nameof(StopVerenigingCollection))]
public class Returns_ArrayOfDetailResponses : End2EndTest<IEnumerable<JObject>>
{
    private readonly StopVerenigingContext _testContext;

    public Returns_ArrayOfDetailResponses(StopVerenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override IEnumerable<JObject> GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekDetailAll(_testContext.CommandResult.Sequence);

    [Fact]
    public void WithVereniging()
        => Response.OnlyTeVerwijderen()
                   .Should()
                   .ContainEquivalentOf(new DetailAllConverter.TeVerwijderenVereniging()
                    {
                        Vereniging = new DetailAllConverter.TeVerwijderenVereniging.TeVerwijderenVerenigingData()
                        {
                            VCode = _testContext.VCode,
                            TeVerwijderen = true,
                            DeletedAt = DateTime.UtcNow.Date.ToInstant().FormatAsBelgianDate(),
                        },
                    });
}
