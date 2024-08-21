namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

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
