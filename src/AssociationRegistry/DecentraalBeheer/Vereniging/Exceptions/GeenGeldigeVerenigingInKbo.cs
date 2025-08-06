namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class GeenGeldigeVerenigingInKbo : DomainException
{
    public GeenGeldigeVerenigingInKbo() : base(ExceptionMessages.GeenGeldigeVerenigingInKbo)
    {
    }

    protected GeenGeldigeVerenigingInKbo(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
