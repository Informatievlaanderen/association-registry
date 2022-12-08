namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using Framework.Helpers;

public class JsonRequestAdminApiFixture : AdminApiFixture
{
    public StringContent Content { get; }

    public JsonRequestAdminApiFixture(string name, string file) : base(name)
    {
        Content = GetJsonRequestBody(file).AsJsonContent();
    }
    private string GetJsonRequestBody(string file)
        => GetType()
            .GetAssociatedResourceJson(file);
}
