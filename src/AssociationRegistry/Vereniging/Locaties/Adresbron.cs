namespace AssociationRegistry.Vereniging;

using Events;
using Exceptions;
using Framework;

public class Adresbron: IAdresbron
{
    public static readonly Adresbron AR = new(code: "AR", beschrijving: "Adressenregister");
    public static readonly Adresbron[] All = { AR };

    public Adresbron(string code, string beschrijving)
    {
        Code = code;
        Beschrijving = beschrijving;
    }

    public string Code { get; }
    public string Beschrijving { get; }

    public static Adresbron Parse(string broncode)
    {
        Throw<BroncodeIsOngeldig>.If(!CanParse(broncode));

        return All.Single(t => t.Code == broncode);
    }

    public static bool CanParse(string broncode)
        => All.Any(t => t.Code == broncode);

    public static implicit operator string(Adresbron verenigingsbron)
        => verenigingsbron.Code;

    public static implicit operator Adresbron(string bronString)
        => Parse(bronString);
}
