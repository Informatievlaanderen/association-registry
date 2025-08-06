namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Security;

public static class ClaimConstants
{
    public static class ClaimTypes
    {
        public const string Scope = "scope";
        public const string ClientId = "client_id";
    }

    public static class Scopes
    {
        public const string Admin = "dv_verenigingsregister_beheer";
    }
}
