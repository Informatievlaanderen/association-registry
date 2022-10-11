namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Runtime.Serialization;
using Events;

[DataContract]
public record VerenigingCreated(
    [property: DataMember] string VNummer,
    [property: DataMember] string Naam
) : IEvent;
