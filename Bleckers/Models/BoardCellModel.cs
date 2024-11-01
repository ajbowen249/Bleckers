namespace Bleckers;
public class BoardCellModel {
    public required Location Location { get; set; }

    public PieceModel? Piece { get; set; } = null;
}
