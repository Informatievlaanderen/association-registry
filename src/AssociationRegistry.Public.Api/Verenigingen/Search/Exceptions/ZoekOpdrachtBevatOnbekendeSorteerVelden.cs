namespace AssociationRegistry.Public.Api.Verenigingen.Search.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System;

[Serializable]
public class ZoekOpdrachtBevatOnbekendeSorteerVelden : DomainException
{
    public ZoekOpdrachtBevatOnbekendeSorteerVelden(string onbekendVeld) :
        base(string.Format(ExceptionMessages.ZoekOpdrachtBevatOnbekendeSorteerVelden, onbekendVeld))
    {
    }
}
