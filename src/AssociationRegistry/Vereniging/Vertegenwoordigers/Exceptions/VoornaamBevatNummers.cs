namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class VoornaamBevatNummers : DomainException
{
    public VoornaamBevatNummers() : base(ExceptionMessages.VoornaamBevatNummers)
    {
    }

    protected VoornaamBevatNummers(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
