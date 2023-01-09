namespace AssociationRegistry.Test.Admin.Api.Fixtures;

using Framework;

public class JsonRequestAdminApiFixture : AdminApiFixture
{
    public string JsonContent { get; }

    protected JsonRequestAdminApiFixture(string name, string file) : base(name)
    {
        JsonContent = GetType()
            .GetAssociatedResourceJson(file);
    }
}
