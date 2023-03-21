namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek;

using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public record HistoriekResponse(
    [property: DataMember(Name = "vCode")] string VCode,
    [property: DataMember(Name = "gebeurtenissen")] List<HistoriekGebeurtenisResponse> Gebeurtenissen);

[DataContract]
public record HistoriekGebeurtenisResponse(
    [property: DataMember(Name = "beschrijving")] string Beschrijving,
    [property: DataMember(Name = "gebeurtenis")] string Gebeurtenis,
    [property: DataMember(Name = "initiator")] string Initiator,
    [property: DataMember(Name = "tijdstip")] string Tijdstip);
