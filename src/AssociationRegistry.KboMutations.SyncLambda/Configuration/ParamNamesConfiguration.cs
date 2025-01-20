namespace AssociationRegistry.KboMutations.SyncLambda.Configuration;

using KboMutations.Configuration;

public class ParamNamesConfiguration: ISlackConfiguration
{
    public static string Section = "ParamNames";

    public DateTime Created => DateTime.Now;
    public string MagdaCertificate { get; set; } = null!;
    public string MagdaCertificatePassword { get; set; } = null!;
    public string PostgresPassword { get; set; } = null!;
    public string SlackWebhook { get; set; } = null;
}