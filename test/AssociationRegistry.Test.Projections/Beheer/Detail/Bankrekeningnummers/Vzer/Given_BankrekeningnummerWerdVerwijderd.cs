namespace AssociationRegistry.Test.Projections.Beheer.Detail.Bankrekeningnummers.Vzer;

using AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdVerwijderd(
    BeheerDetailScenarioFixture<BankrekeningnummerWerdVerwijderdScenario> fixture
) : BeheerDetailScenarioClassFixture<BankrekeningnummerWerdVerwijderdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Vertegenwoordiger_Is_Verwijderd()
    {
        fixture
            .Result.Bankrekeningnummers.SingleOrDefault(x =>
                x.BankrekeningnummerId == fixture.Scenario.BankrekeningnummerWerdVerwijderd.BankrekeningnummerId
            )
            .Should()
            .BeNull();
    }
}
