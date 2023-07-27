﻿namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class UnsupportedOperationForVerenigingstype : DomainException
{
    public UnsupportedOperationForVerenigingstype() : base("Deze actie kan niet uitgevoerd worden op dit type vereniging.")
    {
    }

    protected UnsupportedOperationForVerenigingstype(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
