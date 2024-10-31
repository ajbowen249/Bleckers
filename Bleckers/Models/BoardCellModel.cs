namespace Bleckers;
public class BoardCellModel {
	public int Row { get; set; } = default!;
	public int Col { get; set; } = default!;

    public PieceModel? Piece { get; set; } = null;
}

