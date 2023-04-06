namespace AssociationRegistry.ContactGegevens.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class UnknownContactgegeven : AggregateNotFoundException
{
    public UnknownContactgegeven() : base($"Het opgegeven contactgegeven bestaat niet of is reeds verwijderd.", typeof(Contactgegeven))
    {
    }

    protected UnknownContactgegeven(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
