namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Publiek.Detail_All;

using Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using Formats;
using Public.Api.Verenigingen.DetailAll;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NodaTime.Extensions;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_ArrayOfDetailResponses : End2EndTest<MarkeerAlsDubbelVanContext, MarkeerAlsDubbelVanRequest, IEnumerable<JObject>>
{
    public override Func<IApiSetup, IEnumerable<JObject>> GetResponse =>
        setup => setup.PublicApiHost.GetPubliekDetailAll();

    public Returns_ArrayOfDetailResponses(MarkeerAlsDubbelVanContext context)
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
