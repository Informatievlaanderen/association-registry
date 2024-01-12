namespace AssociationRegistry.Vereniging.TelefoonNummers.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class TelefoonNummerMoetCijferBevatten : DomainException
{
    public TelefoonNummerMoetCijferBevatten() : base(ExceptionMessages.NoNumbersInTelefoonNummer)
    {
    }

    protected TelefoonNummerMoetCijferBevatten(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
