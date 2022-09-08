using Xunit;

namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

[CollectionDefinition(Name)]
public class VerenigingPublicApiCollection :
    ICollectionFixture<VerenigingPublicApiFixture>
{
    public const string Name = "Vereniging public api collection";
}
[CollectionDefinition(Name)]
public class VerenigingPublicApiWith72VerenigingenCollection :
    ICollectionFixture<VerenigingPublicApiFixtureWith72Verenigingen>
{
    public const string Name = "Vereniging public api collection with 72 verenigingen";
}
