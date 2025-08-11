namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;

using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingUitKbo;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

[DataContract]
public class RegistreerVerenigingUitKboRequest
{
    /// <summary>Kbo nummer van de vereniging</summary>
    [DataMember]
    [Required]
    public string KboNummer { get; init; } = null!;

    public RegistreerVerenigingUitKboCommand ToCommand()
        => new(DecentraalBeheer.Vereniging.KboNummer.Create(KboNummer));
}
