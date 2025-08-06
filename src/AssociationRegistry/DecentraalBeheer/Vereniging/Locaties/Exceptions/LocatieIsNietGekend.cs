namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

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
