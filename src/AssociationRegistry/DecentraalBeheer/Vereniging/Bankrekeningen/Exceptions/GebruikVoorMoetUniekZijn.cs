namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;

using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class GebruikVoorMoetUniekZijn: DomainException
{
    public GebruikVoorMoetUniekZijn() : base(ExceptionMessages.DoelMoetUniekZijn)
    {
    }

    protected GebruikVoorMoetUniekZijn(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
