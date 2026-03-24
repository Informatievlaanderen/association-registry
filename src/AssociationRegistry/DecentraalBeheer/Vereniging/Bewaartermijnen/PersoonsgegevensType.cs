namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;

using AssociationRegistry.Framework;
using Exceptions;

public record PersoonsgegevensType
{
    public string Value { get; }

    public static PersoonsgegevensType Vertegenwoordigers = new("Vertegenwoordigers");

    public static PersoonsgegevensType[] All =
    [
        Vertegenwoordigers,
    ];

    private PersoonsgegevensType(string persoonsgegevensType)
    {
        Value = persoonsgegevensType;
    }


    public static PersoonsgegevensType Create(string value)
    {
        var bewaartermijnType = All.SingleOrDefault(x => x.Value == value);
        Throw<OngeldigBewaartermijnType>.If(bewaartermijnType == null);

        return bewaartermijnType!;
    }
}
