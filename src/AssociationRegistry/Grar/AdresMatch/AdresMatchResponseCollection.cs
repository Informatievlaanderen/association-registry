namespace AssociationRegistry.Grar.AdresMatch;

using AssociationRegistry.Grar.Models;
using System.Collections.ObjectModel;

public class AdresMatchResponseCollection : ReadOnlyCollection<AddressMatchResponse>
{
    public static bool HasPerfectScore(AddressMatchResponse response)
        => response.Score == AddressMatchResponse.PerfectScore;

    public bool HasNoResponse => Count == 0;

    public AddressMatchResponse? Singular100ScoreResponse => this.Count(HasPerfectScore) == 1 ? this.Single(HasPerfectScore) : null;

    public AdresMatchResponseCollection(IList<AddressMatchResponse> list) : base(list)
    {
    }
}
