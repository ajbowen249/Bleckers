namespace Bleckers;

public class GameModel {
	public List<List<BoardCellModel>> Board = new List<List<BoardCellModel>>();
	
	public event EventHandler? StateChanged;
	public AutoProp<Faction> FactionTurn = new AutoProp<Faction>(Faction.Black);
	public AutoProp<PieceModel?> SelectedPiece = new AutoProp<PieceModel?>(null);

	public Tuple<int, int>? SelectedLocation { get; private set; } = null;

	public GameModel() {
		FactionTurn.ValueChanged += (s, v) => OnStateChanged();
		SelectedPiece.ValueChanged += (s, v) => {
			SelectedLocation = v == null ? null : FindPiece(v);
			OnStateChanged();
		};

		var nextPieceId = 0;

		for (int row = 0; row < Constants.BOARD_HEIGHT; row++) {
			var cellRow = new List<BoardCellModel>();

			for (int col = 0; col < Constants.BOARD_WIDTH; col++) {
				var cellModel = new BoardCellModel { Row = row, Col = col };
				var addPiece = false;

				if (row != 3 && row != 4) {
					if (row % 2 != 0) {
						addPiece = col % 2 == 0;
					} else {
						addPiece = col % 2 != 0;
					}
				}

				if (addPiece) {
					var faction = row < 3 ? Faction.Black : Faction.Red;

					var piece = new PieceModel {
						ID = nextPieceId++,
						Faction = faction,
						IsAlive = true,
					};

					cellModel.Piece = piece;
				}

				cellRow.Add(cellModel);
			}

			Board.Add(cellRow);
		}
	}

	private void OnStateChanged() {
		StateChanged?.Invoke(this, EventArgs.Empty);
	}

	private Tuple<int, int> FindPiece(PieceModel piece) {
		for (int row = 0; row < Board.Count; row++) {
			for (int col = 0; col < Board[0].Count; col++) {
				if (Board[row][col].Piece == piece) {
					return new Tuple<int, int>(row, col);
				}
			}
		}

		throw new Exception("Piece not found on board.");
	}
}
