namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;

using AssociationRegistry.Framework;
using Exceptions;

public record BewaartermijnType
{
    public string Value { get; }

    public static BewaartermijnType Vertegenwoordigers = new("Vertegenwoordigers");

    public static BewaartermijnType[] All =
    [
        Vertegenwoordigers,
    ];

    private BewaartermijnType(string bewaartermijnType)
    {
        Value = bewaartermijnType;
    }


    public static BewaartermijnType Create(string value)
    {
        var bewaartermijnType = All.SingleOrDefault(x => x.Value == value);
        Throw<OngeldigBewaartermijnType>.If(bewaartermijnType == null);

        return bewaartermijnType!;
    }
}
