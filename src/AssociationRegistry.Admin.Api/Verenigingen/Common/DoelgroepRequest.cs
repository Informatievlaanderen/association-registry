namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using System.Runtime.Serialization;
using Vereniging;

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
    public int? Minimumleeftijd { get; set; }

    /// <summary>
    /// De maximum leeftijd voor de doelgroep
    /// </summary>
    [DataMember]
    public int? Maximumleeftijd { get; set; }

    public static Doelgroep Map(DoelgroepRequest? request)
    {
        if(request is null)
            return Doelgroep.Null;

        return Doelgroep.Create(request.Minimumleeftijd, request.Maximumleeftijd);
    }
}
