namespace AssociationRegistry.Test.Public.Api.Public.Api.Fixtures;

using Xunit;

[CollectionDefinition(Name)]
public class VerenigingPublicApiCollection :
    ICollectionFixture<StaticPublicApiFixture>
{
    public const string Name = "Vereniging public api collection";
}
