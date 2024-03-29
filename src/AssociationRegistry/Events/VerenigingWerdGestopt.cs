﻿namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VerenigingWerdGestopt(DateOnly Einddatum) : IEvent
{
    public static VerenigingWerdGestopt With(Datum einddatum)
        => new(einddatum.Value);
}
