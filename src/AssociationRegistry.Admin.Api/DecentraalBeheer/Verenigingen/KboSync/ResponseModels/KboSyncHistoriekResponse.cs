namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.KboSync.ResponseModels;

using System.Collections.ObjectModel;
using System.Runtime.Serialization;

/// <summary>Alle gebeurtenissen van deze vereniging</summary>
[DataContract]
public class KboSyncHistoriekResponse : ReadOnlyCollection<KboSyncHistoriekGebeurtenisResponse>
{
    public KboSyncHistoriekResponse(IList<KboSyncHistoriekGebeurtenisResponse> gebeurtenissen) : base(gebeurtenissen)
    {
    }
}
