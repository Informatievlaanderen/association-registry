namespace AssociationRegistry.Public.Schema.Constants;

public class GuidNamespace
{
    private const string _vereniging = "1039DC21-78ED-40E4-AF88-22306F4C8FEB";
    public static Guid Vereniging => Guid.Parse(_vereniging);
    private const string _hoofdactiviteit = "D45897A0-8BA1-49B8-A2F3-31AEB0DEF89C";
    public static Guid Hoofdactiviteit => Guid.Parse(_hoofdactiviteit);
}
