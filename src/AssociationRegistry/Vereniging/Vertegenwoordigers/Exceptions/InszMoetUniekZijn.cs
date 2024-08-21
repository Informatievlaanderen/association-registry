namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
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
