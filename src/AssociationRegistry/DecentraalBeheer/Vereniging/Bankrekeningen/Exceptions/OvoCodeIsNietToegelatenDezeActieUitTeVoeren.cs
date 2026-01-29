namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class OvoCodeIsNietToegelatenDezeActieUitTeVoeren : DomainException
{
    public OvoCodeIsNietToegelatenDezeActieUitTeVoeren(string ovoCode)
        : base(string.Format(ExceptionMessages.OvoCodeIsNietGemachtigdOmDezeActieUitTeVoeren, ovoCode))
    {
    }

    protected OvoCodeIsNietToegelatenDezeActieUitTeVoeren(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
