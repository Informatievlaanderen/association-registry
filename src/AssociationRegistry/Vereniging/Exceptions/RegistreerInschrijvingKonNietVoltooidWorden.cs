namespace AssociationRegistry.Vereniging.Exceptions;

using Resources;
using System.Runtime.Serialization;

[Serializable]
public class RegistreerInschrijvingKonNietVoltooidWorden : ApplicationException
{
    public RegistreerInschrijvingKonNietVoltooidWorden() : base(ExceptionMessages.RegistreerInschrijvingKonNietVoltooidWorden)
    {
    }

    protected RegistreerInschrijvingKonNietVoltooidWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
