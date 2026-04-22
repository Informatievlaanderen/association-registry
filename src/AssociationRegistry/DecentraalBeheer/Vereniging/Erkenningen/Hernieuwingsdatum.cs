namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using Exceptions;
using Primitives;

public record Hernieuwingsdatum
{
    public DateOnly? Value { get; }

    private Hernieuwingsdatum(DateOnly? value)
    {
        Value = value;
    }

    public static Hernieuwingsdatum Create(DateOnly? hernieuwingsDatum, ErkenningsPeriode erkenningsPeriode)
    {
        if (hernieuwingsDatum.HasValue && !IsValid(hernieuwingsDatum.Value, erkenningsPeriode))
            throw new HernieuwingsDatumMoetTussenStartEnEindDatumLiggen();

        return new Hernieuwingsdatum(hernieuwingsDatum);
    }

    public static Hernieuwingsdatum Hydrate(DateOnly? hernieuwingsDatum) => new(hernieuwingsDatum);

    private static bool IsValid(DateOnly hernieuwingsDatum, ErkenningsPeriode periode) =>
        IsBetweenStartAndEnd(hernieuwingsDatum, periode);

    private static bool IsBetweenStartAndEnd(DateOnly hernieuwingsDatum, ErkenningsPeriode erkenningsPeriode) =>
        (!erkenningsPeriode.Startdatum.HasValue || hernieuwingsDatum >= erkenningsPeriode.Startdatum.Value)
        && (!erkenningsPeriode.Einddatum.HasValue || hernieuwingsDatum <= erkenningsPeriode.Einddatum.Value);
}
