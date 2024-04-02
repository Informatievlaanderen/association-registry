namespace AssociationRegistry.Acm.Api.Constants;

public static class Security
{
    public static class ClaimTypes
    {
        public const string Scope = "scope";
        public const string ClientId = "client_id";
    }

    public static class Scopes
    {
        public const string ACM = "dv_verenigingsregister_hoofdvertegenwoordigers";
        public const string Info = "vo_info";
        public const string Admin = "dv_verenigingsregister_beheer";
    }
}
