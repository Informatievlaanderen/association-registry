namespace AssociationRegistry.Grar.AdresMatch;

using AssociationRegistry.Grar.Models;
using Contracts;

public interface IAdresMatchStrategy
{
    AddressMatchResponse? DetermineMatch(AdresMatchResponseCollection responses);
}

public class PerfectScoreMatchStrategy : IAdresMatchStrategy
{
    public AddressMatchResponse? DetermineMatch(AdresMatchResponseCollection responses)
    {
        if (responses.HasNoResponse)
            return null;

        // Returns the single 100-score response, or null if there are multiple or none
        return responses.Singular100ScoreResponse;
    }
}
