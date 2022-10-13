namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using Xunit;

[CollectionDefinition(Name)]
public class VerenigingDbCollection : ICollectionFixture<VerenigingDbFixture>
{
    public const string Name = "Vereniging admin db collection";
}
