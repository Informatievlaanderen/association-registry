namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class IpdcProductNummerOntbreekt : DomainException
{
    public IpdcProductNummerOntbreekt() : base(ExceptionMessages.IpdcProductNummerRequired)
    {
    }

    protected IpdcProductNummerOntbreekt(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
