namespace AssociationRegistry.ContactGegevens.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DuplicateContactgegeven : DomainException
{
    public DuplicateContactgegeven(string type) : base($"Er is reeds een {type} contactgegeven met de opgegeven waarde.")
    {
    }

    protected DuplicateContactgegeven(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class UnknownContactgegeven : DomainException
{
    public UnknownContactgegeven() : base($"Het opgegeven contactgegeven bestaat niet of is reeds verwijderd.")
    {
    }

    protected UnknownContactgegeven(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
