namespace AssociationRegistry.Vereniging;

using Exceptions;
using Framework;

public record Doelgroep
{
    public static readonly Doelgroep Null = Create(StandaardMinimumleeftijd, StandaardMaximumleeftijd);
    public const int StandaardMinimumleeftijd = 0;
    public const int StandaardMaximumleeftijd = 150;

    public int Minimumleeftijd { get; init; }
    public int Maximumleeftijd { get; init; }

    public static Doelgroep Create(int? minimumleeftijd, int? maximumleeftijd)
    {
        var min =minimumleeftijd ?? StandaardMinimumleeftijd;
        var max = maximumleeftijd ?? StandaardMaximumleeftijd;

        Throw<DoelgroepOutOfRange>.If(IsBelowStandaard(min));
        Throw<DoelgroepOutOfRange>.If(IsAboveStandaard(min));
        Throw<DoelgroepOutOfRange>.If(IsBelowStandaard(max));
        Throw<DoelgroepOutOfRange>.If(IsAboveStandaard(max));

        Throw<InvalidDoelgroepRange>.If(min > max);

        return new Doelgroep
        {
            Minimumleeftijd = min,
            Maximumleeftijd = max,
        };
    }

    private static bool IsAboveStandaard(int leeftijd)
        => leeftijd > StandaardMaximumleeftijd;


    private static bool IsBelowStandaard(int leeftijd)
        => leeftijd < StandaardMinimumleeftijd;
}
