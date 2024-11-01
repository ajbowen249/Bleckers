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

    public bool IsInBoard() {
        return row >= 0 && col >= 0 && row < Constants.BOARD_HEIGHT && col < Constants.BOARD_WIDTH;
    }

    public static Location operator +(Location left, Location right) {
        return new Location(left.row + right.row, left.col + right.col);
    }
}
