﻿namespace AssociationRegistry.Vereniging;

using Events;
using GemeentenaamDecorator;

public record Gemeentenaam(string Naam)
{
    public static Gemeentenaam Hydrate(string gemeente)
        => new(gemeente);

    public static Gemeentenaam FromVerrijkteGemeentenaam(VerrijkteGemeentenaam gemeentenaam)
        => new(gemeentenaam.Format());

    public override string ToString()
        => Naam;
}
