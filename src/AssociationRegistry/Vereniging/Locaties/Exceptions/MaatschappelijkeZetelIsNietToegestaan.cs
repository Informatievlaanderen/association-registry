namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MaatschappelijkeZetelIsNietToegestaan : DomainException
{
    public MaatschappelijkeZetelIsNietToegestaan() : base(ExceptionMessages.MaatschappelijkeZetelIsNotAllowed)
    {
    }

    protected MaatschappelijkeZetelIsNietToegestaan(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
