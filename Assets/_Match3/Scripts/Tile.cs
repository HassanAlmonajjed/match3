public class Tile : System.IEquatable<Tile>
{
    public int id;

    public override string ToString()
    {
        return id.ToString();
    }

    public bool Equals(Tile other)
    {
        if (other == null)
            return false;

        if (id < 0 || other.id < 0)
            return false;

        return id == other.id;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Tile);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public static bool operator ==(Tile left, Tile right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(Tile left, Tile right)
    {
        return !(left == right);
    }
}
