namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class AndereVerenigingIsVerwijderd : DomainException
{

    public AndereVerenigingIsVerwijderd() : base(ExceptionMessages.AndereVerenigingIsVerwijderd)
    { }

    protected AndereVerenigingIsVerwijderd(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
