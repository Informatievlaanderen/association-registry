namespace AssociationRegistry.OpenTelemetry;

public interface IInstrumentation
{
    public string ActivitySourceName { get; }
    public string MeterName { get; }
}
