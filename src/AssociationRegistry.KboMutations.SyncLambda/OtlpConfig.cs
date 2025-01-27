namespace AssociationRegistry.KboMutations.SyncLambda;

public class OtlpConfig
{
    public string AuthHeader { get; set; }
    public string MetricsUri { get; set; }
    public string TracingUri { get; set; }
    public string LogsUri { get; set; }
}
public class OtlpConfigs
{
    public Dictionary<string, OtlpConfig> Configs { get; set; } = new();
}
