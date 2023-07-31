namespace AssociationRegistry.Kbo;

using Magda;
using Vereniging;

public class VerenigingVolgensKbo
{
    public KboNummer KboNummer { get; init; } = null!;
    public Rechtsvorm Rechtsvorm { get; set; } = null!;
}
