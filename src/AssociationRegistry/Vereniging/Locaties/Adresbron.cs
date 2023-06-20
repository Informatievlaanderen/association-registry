namespace AssociationRegistry.Vereniging;

public class Adresbron
{
    public static readonly Adresbron AR = new("AR", "Adressenregister");

    public static readonly Adresbron[] All = { AR };

    public Adresbron(string code, string beschrijving)
    {
        Code = code;
        Beschrijving = beschrijving;
    }

    public string Code { get; }
    public string Beschrijving { get; }

    public static Adresbron Parse(string waarde)
        => All.Single(t => t.Code == waarde);

    public static implicit operator string(Adresbron verenigingsbron)
        => verenigingsbron.Code;

    public static implicit operator Adresbron(string bronString)
        => Parse(bronString);
}
