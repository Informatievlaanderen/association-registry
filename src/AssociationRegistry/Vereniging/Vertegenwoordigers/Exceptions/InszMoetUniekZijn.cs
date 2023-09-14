namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
