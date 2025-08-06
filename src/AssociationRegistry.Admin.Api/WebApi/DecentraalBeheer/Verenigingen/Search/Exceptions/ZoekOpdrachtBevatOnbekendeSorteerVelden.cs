namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.Exceptions;

using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class ZoekOpdrachtBevatOnbekendeSorteerVelden : DomainException
{
    public ZoekOpdrachtBevatOnbekendeSorteerVelden(string onbekendVeld) :
        base(string.Format(ExceptionMessages.ZoekOpdrachtBevatOnbekendeSorteerVelden, onbekendVeld))
    {
    }
}
