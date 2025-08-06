namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

public class LidmaatschapMagNietVerwijzenNaarEigenVereniging : DomainException
{
    public LidmaatschapMagNietVerwijzenNaarEigenVereniging() : base(ExceptionMessages.LidmaatschapMagNietVerwijzenNaarEigenVereniging)
    {
    }

    protected LidmaatschapMagNietVerwijzenNaarEigenVereniging(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
