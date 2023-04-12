namespace AssociationRegistry.Contactgegevens.TelefoonNummers.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class NoNumbersInTelefoonNummer : DomainException
{
    public NoNumbersInTelefoonNummer() : base("TelefoonNummer moet minstens één cijfer bevatten")
    {
    }

    protected NoNumbersInTelefoonNummer(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
