namespace AssociationRegistry.Acm.Api.Caches;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IVerenigingenRepository
{
    VerenigingenPerRijksregisternummer Verenigingen { get; set; }
    Task UpdateVerenigingen(VerenigingenAsDictionary verenigingenAsDictionary, Stream verenigingenStream, CancellationToken cancellationToken);
}
