namespace AssociationRegistry.Test.Admin.Api.Fixtures;

using Framework;

public abstract class JsonRequestAdminApiFixture : AdminApiFixture2
{
    public string JsonContent { get; }

    protected JsonRequestAdminApiFixture(string name, string file) : base(name)
    {
        JsonContent = GetType()
            .GetAssociatedResourceJson(file);
    }
}
