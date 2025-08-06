namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
