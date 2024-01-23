namespace AssociationRegistry.Admin.Api.Verenigingen.Search.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System;

[Serializable]
public class ZoekOpdrachtBevatOnbekendeSorteerVelden : DomainException
{
    public ZoekOpdrachtBevatOnbekendeSorteerVelden(string onbekendVeld) :
        base(string.Format(ExceptionMessages.ZoekOpdrachtBevatOnbekendeSorteerVelden, onbekendVeld))
    {
    }
}
