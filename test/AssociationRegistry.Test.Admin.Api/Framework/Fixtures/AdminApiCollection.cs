namespace AssociationRegistry.Test.Admin.Api.Framework.Fixtures;

using Xunit;

[CollectionDefinition(nameof(AdminApiCollection))]
public class AdminApiCollection : ICollectionFixture<EventsInDbScenariosFixture>
{
}

[CollectionDefinition(nameof(AdminApiScenarioCollection))]
public class AdminApiScenarioCollection : ICollectionFixture<AdminApiScenarioFixture>
{
}
