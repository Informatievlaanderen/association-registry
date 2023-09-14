namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class LocatieIsNietGekend : DomainException
{
    public LocatieIsNietGekend(string locatieId) : base($"Locatie met locatieId '{locatieId}' is niet gekend")
    {
    }

    protected LocatieIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
