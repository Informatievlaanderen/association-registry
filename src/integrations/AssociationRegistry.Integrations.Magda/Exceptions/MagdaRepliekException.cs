namespace AssociationRegistry.Integrations.Magda.Exceptions;

using Repertorium.RegistreerInschrijving;
using System.Text;

[Serializable]
public class MagdaRepliekException : Exception
{
    public MagdaRepliekException(IEnumerable<UitzonderingType> uitzonderingFouten) : base(BuildMessage(uitzonderingFouten))
    {
    }

    private static string BuildMessage(IEnumerable<UitzonderingType> uitzonderingFouten)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("De volgende fouten hebben zich voorgedaan bij het aanroepen van de Magda RegistreerInschrijvingDienst.");

        foreach (var uitzonderingType in uitzonderingFouten)
        {
            stringBuilder.AppendLine(uitzonderingType.Diagnose);
        }

        return stringBuilder.ToString();
    }
}
