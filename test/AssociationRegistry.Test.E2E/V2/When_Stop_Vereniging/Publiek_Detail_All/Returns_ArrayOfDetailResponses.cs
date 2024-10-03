namespace AssociationRegistry.Test.E2E.V2.When_Stop_Vereniging.Publiek_Detail_All;

using Admin.Api.Verenigingen.Stop.RequestModels;using Data;
using Framework.AlbaHost;
using FluentAssertions;
using Framework.ApiSetup;
using Framework.TestClasses;
using Newtonsoft.Json.Linq;
using Public.Api.Verenigingen.Detail;
using Public.Api.Verenigingen.Detail.ResponseModels;
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
                   .ContainEquivalentOf(new ResponseWriter.TeVerwijderenVereniging()
                    {
                        Vereniging = new ResponseWriter.TeVerwijderenVereniging.TeVerwijderenVerenigingData()
                        {
                            VCode = Context.VCode,
                            TeVerwijderen = true,
                        },
                    });
}
