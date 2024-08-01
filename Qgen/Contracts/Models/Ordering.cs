namespace Qgen.Contracts.Models;

public class Ordering
{
    public Ordering(string name, bool ascending)
    {
        Name = name;
        Ascending = ascending;
    }

    public string Name { get; }
    public bool Ascending { get; }
}
