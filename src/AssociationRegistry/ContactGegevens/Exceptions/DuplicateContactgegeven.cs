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
