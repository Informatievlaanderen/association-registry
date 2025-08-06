namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Framework;
using Exceptions;

public record Doelgroep
{
    public static readonly Doelgroep Null = Create(StandaardMinimumleeftijd, StandaardMaximumleeftijd);
    public const int StandaardMinimumleeftijd = 0;
    public const int StandaardMaximumleeftijd = 150;
    public int Minimumleeftijd { get; init; }
    public int Maximumleeftijd { get; init; }

    private Doelgroep()
    {
    }

    public static Doelgroep Create(int? minimumleeftijd, int? maximumleeftijd)
    {
        var min = minimumleeftijd ?? StandaardMinimumleeftijd;
        var max = maximumleeftijd ?? StandaardMaximumleeftijd;

        Throw<DoelgroepValtBuitenToegestaneWaarden>.If(IsBelowStandaard(min));
        Throw<DoelgroepValtBuitenToegestaneWaarden>.If(IsAboveStandaard(min));
        Throw<DoelgroepValtBuitenToegestaneWaarden>.If(IsBelowStandaard(max));
        Throw<DoelgroepValtBuitenToegestaneWaarden>.If(IsAboveStandaard(max));

        Throw<MinimumLeeftijdMoetKleinerOfGelijkZijnAanMaximumLeeftijd>.If(min > max);

        return new Doelgroep
        {
            Minimumleeftijd = min,
            Maximumleeftijd = max,
        };
    }

    public static Doelgroep Hydrate(int doelgroepMinimumleeftijd, int doelgroepMaximumleeftijd)
        => new()
        {
            Minimumleeftijd = doelgroepMinimumleeftijd,
            Maximumleeftijd = doelgroepMaximumleeftijd,
        };

    public static bool Equals(Doelgroep? oudeDoelgroep, Doelgroep? nieuweDoelgroep)
    {
        if (oudeDoelgroep is null && nieuweDoelgroep is null) return true;

        return oudeDoelgroep is not null &&
               nieuweDoelgroep is not null &&
               nieuweDoelgroep.Minimumleeftijd == oudeDoelgroep.Minimumleeftijd &&
               nieuweDoelgroep.Maximumleeftijd == oudeDoelgroep.Maximumleeftijd;
    }

    private static bool IsAboveStandaard(int leeftijd)
        => leeftijd > StandaardMaximumleeftijd;

    private static bool IsBelowStandaard(int leeftijd)
        => leeftijd < StandaardMinimumleeftijd;
}
