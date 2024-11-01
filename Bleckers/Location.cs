namespace Bleckers;

public class Location {
    public int row;
    public int col;

    public Location(int row, int col) {
        this.row = row;
        this.col = col;
    }

    public bool Equals(Location other) {
        return row == other.row && col == other.col;
    }
}
