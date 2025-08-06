namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public class CodeerSysteem
{
    public static readonly CodeerSysteem KBO = new(waarde: "KBO nummer");
    public static readonly CodeerSysteem VR = new(waarde: "Vcode");

    public CodeerSysteem(string waarde)
    {
        Waarde = waarde;
    }

    public string Waarde { get; }

    public static implicit operator string(CodeerSysteem sleutelbron)
        => sleutelbron.Waarde;
}
