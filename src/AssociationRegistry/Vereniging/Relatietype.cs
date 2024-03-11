namespace AssociationRegistry.Vereniging;

public class Relatietype
{
    public Relatietype(string beschrijving, string inverseBeschrijving)
    {
        Beschrijving = beschrijving;
        InverseBeschrijving = inverseBeschrijving;
    }

    public string Beschrijving { get; }
    public string InverseBeschrijving { get; }
}
