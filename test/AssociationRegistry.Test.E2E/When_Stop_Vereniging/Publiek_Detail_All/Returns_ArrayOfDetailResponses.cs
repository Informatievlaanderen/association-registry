namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Publiek_Detail_All;

using Admin.Api.Verenigingen.Stop.RequestModels;
using Alba;
using Data;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(StopVerenigingPublicCollection.Name)]
public class Returns_ArrayOfDetailResponses(StopVerenigingContext<PublicApiSetup> context)
    : End2EndTest<StopVerenigingContext<PublicApiSetup>, StopVerenigingRequest, TeVerwijderenVereniging[]>(
        context)
{
    protected override Func<IAlbaHost, TeVerwijderenVereniging[]> GetResponse =>
        host => host.GetPubliekDetailAll<TeVerwijderenVereniging>();

    [Fact]
    public void WithVereniging()
        => Response.Single().Should().BeEquivalentTo(new TeVerwijderenVereniging
        {
            Vereniging = new TeVerwijderenVerenigingData()
            {
                VCode = VCode,
                TeVerwijderen = true,
            },
        });
}
