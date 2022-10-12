namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Runtime.Serialization;
using Events;

[DataContract]
public record VerenigingWerdGeregistreerd(
    [property: DataMember] string VCode,
    [property: DataMember] string Naam
) : IEvent;
