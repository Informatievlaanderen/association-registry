namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class InszMoetUniekZijn : DomainException
{
    public InszMoetUniekZijn() : base(ExceptionMessages.DuplicateInszProvided)
    {
    }

    protected InszMoetUniekZijn(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
