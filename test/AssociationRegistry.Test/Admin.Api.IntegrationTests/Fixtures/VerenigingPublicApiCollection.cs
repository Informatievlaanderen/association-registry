namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using Xunit;

[CollectionDefinition(Name)]
public class VerenigingAdminApiCollection : ICollectionFixture<VerenigingAdminApiFixture>
{
    public const string Name = "Vereniging admin api collection";
}
