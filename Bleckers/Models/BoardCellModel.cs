namespace Bleckers;
public class BoardCellModel {
    public Location Location { get; set; } = default!;

    public PieceModel? Piece { get; set; } = null;
}

