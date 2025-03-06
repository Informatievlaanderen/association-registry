namespace AssociationRegistry.Test.Admin.Api.Framework.Fixtures.MinimalApi;

using System.Threading.Tasks;

public class MinimalAdminApiFixture : AdminApiFixture
{
    public MinimalAdminApiFixture(): base("minimal")
    {

    }

    protected override Task Given()
        => Task.CompletedTask;
}
