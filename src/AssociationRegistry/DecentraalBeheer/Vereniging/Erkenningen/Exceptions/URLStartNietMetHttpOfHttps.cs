namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class URLStartNietMetHttpOfHttps : DomainException
{
    public URLStartNietMetHttpOfHttps() : base(ExceptionMessages.UrlDoesNotStartWithHttpOrHttps)
    {
    }

    protected URLStartNietMetHttpOfHttps(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
