namespace AssociationRegistry.Vereniging.TelefoonNummers.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class NoNumbersInTelefoonNummer : DomainException
{
    public NoNumbersInTelefoonNummer() : base(ExceptionMessages.NoNumbersInTelefoonNummer)
    {
    }

    protected NoNumbersInTelefoonNummer(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
