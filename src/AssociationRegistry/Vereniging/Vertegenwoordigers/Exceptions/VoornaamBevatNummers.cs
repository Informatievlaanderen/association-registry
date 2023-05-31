namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class VoornaamBevatNummers : DomainException
{
    public VoornaamBevatNummers() : base("Voornaam mag geen nummers bevatten.")
    {
    }

    protected VoornaamBevatNummers(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}