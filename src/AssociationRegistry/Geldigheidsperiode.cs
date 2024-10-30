namespace AssociationRegistry;

using System;
using Vereniging.Exceptions;

public class Geldigheidsperiode
{
    public GeldigVan Van { get; }
    public GeldigTot Tot { get; }

    public bool HasFixedStart => !Van.IsInfinite;
    public bool HasFixedEnd => !Tot.IsInfinite;

    public Geldigheidsperiode() { }

    public Geldigheidsperiode(GeldigVan van, GeldigTot tot)
    {
        var endDate = tot.DateOnly;
        var startDate = van.DateOnly;
        if (endDate < startDate)
            throw new StartdatumLigtNaEinddatum();

        Van = van;
        Tot = tot;
    }

    public static Geldigheidsperiode Infinity
        => new();

    public bool OverlapsWith(Geldigheidsperiode? geldigheidsperiode)
    {
        if (geldigheidsperiode == null)
            return false;

        var periodEndDate = geldigheidsperiode.Tot.DateOnly;
        var startDate = Van.DateOnly;
        if (periodEndDate < startDate)
            return false;

        var endDate = Tot.DateOnly;
        var periodStartDate = geldigheidsperiode.Van.DateOnly;
        if (endDate < periodStartDate)
            return false;

        return true;
    }

    public bool OverlapsWith(DateOnly date)
        => OverlapsWith(
            new Geldigheidsperiode(
                new GeldigVan(date),
                new GeldigTot(date)));

    public override string ToString()
        => $"{Van} => {Tot}";
}
