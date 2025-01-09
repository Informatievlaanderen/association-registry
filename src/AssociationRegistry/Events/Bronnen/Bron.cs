// ReSharper disable once CheckNamespace
namespace AssociationRegistry.Vereniging.Bronnen;

public class Bron
{
    public static readonly Bron Initiator = new("Initiator");
    public static readonly Bron KBO = new("KBO");
    public string Waarde { get; }

    private Bron(string waarde)
    {
        Waarde = waarde;
    }

    public static implicit operator string(Bron bron)
        => bron.Waarde;
}
