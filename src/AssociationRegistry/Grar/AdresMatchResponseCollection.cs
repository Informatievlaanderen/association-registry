namespace AssociationRegistry.Grar;

using Models;
using System.Collections.ObjectModel;

public class AdresMatchResponseCollection : ReadOnlyCollection<AddressMatchResponse>
{
    public bool HasSingularResponse => this.Count == 1 || this.Count(c => c.Score == 100) == 1;
    public bool HasNoResponse => this.Count == 0;

    public AddressMatchResponse SingularResponse {
        get
        {
            if (this.Count == 1)
                return this.Single();

            if (this.Count(c => c.Score == 100) == 1)
                return this.Single(c => c.Score == 100);

            throw new NotSupportedException();
        }
    }

    public AdresMatchResponseCollection(IList<AddressMatchResponse> list) : base(list)
    {
    }
}
