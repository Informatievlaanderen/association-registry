namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Ipdc;

using System.Runtime.Serialization;
using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
