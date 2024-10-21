namespace AssociationRegistry.AcmBevraging;

public record VerenigingenPerKbo(string KboNummer, string VCode, bool IsHoofdvertegenwoordiger)
{
    public static VerenigingenPerKbo NietVanToepassing(string kboNummer)
        => new (kboNummer, VCodeUitzonderingen.NietVanToepassing, false);

    public static VerenigingenPerKbo NogNietBekend(string kboNummer)
        => new (kboNummer, VCodeUitzonderingen.NogNietBekend, false);

    public static VerenigingenPerKbo Bekend(string kboNummer, string vCode)
        => new(kboNummer, vCode, true);

    public static class VCodeUitzonderingen
    {
        public const string NietVanToepassing = "NVT";
        public const string NogNietBekend = "NNB";
    }
};
