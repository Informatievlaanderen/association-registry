namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

/// <summary>
/// De doelgroep waar de activiteiten van deze vereniging zich op concentreert
/// </summary>
[DataContract]
public class DoelgroepRequest
{
    /// <summary>
    /// De minimum leeftijd voor de doelgroep
    /// </summary>
    [DataMember]
    [Range(Doelgroep.StandaardMinimumleeftijd, Doelgroep.StandaardMaximumleeftijd)]
    [DefaultValue(Doelgroep.StandaardMinimumleeftijd)]
    public int? Minimumleeftijd { get; set; }

    /// <summary>
    /// De maximum leeftijd voor de doelgroep
    /// </summary>
    [DataMember]
    [Range(Doelgroep.StandaardMinimumleeftijd, Doelgroep.StandaardMaximumleeftijd)]
    [DefaultValue(Doelgroep.StandaardMaximumleeftijd)]
    public int? Maximumleeftijd { get; set; }

    public static Doelgroep Map(DoelgroepRequest? request)
    {
        if (request is null)
            return Doelgroep.Null;

        return Doelgroep.Create(request.Minimumleeftijd, request.Maximumleeftijd);
    }
}
