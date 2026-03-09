namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Exceptions;

using System.Runtime.Serialization;
using AssociationRegistry.Resources;

[Serializable]
public class OngeldigBewaartermijnType : ApplicationException
{
    public OngeldigBewaartermijnType() : base(ExceptionMessages.OngeldigBewaartermijnType)
    {
    }
    protected OngeldigBewaartermijnType(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
