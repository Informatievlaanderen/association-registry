namespace AssociationRegistry.Test.Projections.Beheer.Detail.Bankrekeningnummers.Kbo;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using Contracts.JsonLdContext;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdVerwijderdUitKBO(
    BeheerDetailScenarioFixture<BankrekeningnummerWerdVerwijderdUitKBOScenario> fixture)
    : BeheerDetailScenarioClassFixture<BankrekeningnummerWerdVerwijderdUitKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Bankrekeningnummer_Is_Verwijderd()
    {
        fixture.Result.Bankrekeningnummers.FirstOrDefault(x => x.Iban == fixture.Scenario.BankrekeningnummerWerdVerwijderdUitKBO.Iban)
               .Should()
               .BeNull();
    }
}
