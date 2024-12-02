namespace AssociationRegistry.Grar.GrarUpdates.Hernummering.Groupers;

using System.Collections.ObjectModel;

public class TeHernummerenStraat : ReadOnlyCollection<TeHernummerenAdres>
{
    public TeHernummerenStraat(IList<TeHernummerenAdres> list) : base(list)
    {
    }

    public string NaarAdresIdVoor(string vanAdresId)
        => this.Single(s => s.VanAdresId.ToString() == vanAdresId).NaarAdresId.ToString();
}
