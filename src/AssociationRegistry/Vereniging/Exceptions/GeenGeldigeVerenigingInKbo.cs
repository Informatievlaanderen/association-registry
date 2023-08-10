namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class GeenGeldigeVerenigingInKbo : DomainException
{
    public GeenGeldigeVerenigingInKbo() : base("Er werd voor dit KBO-nummer geen geldige vereniging gevonden.")
    {
    }

    protected GeenGeldigeVerenigingInKbo(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
