namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Publiek.Detail_All;

using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Newtonsoft.Json.Linq;
using NodaTime.Extensions;
using Public.Api.WebApi.Verenigingen.DetailAll;
using Xunit;

[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<IEnumerable<JObject>>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext) : base(testContext.ApiSetup)
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
