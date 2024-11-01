namespace Bleckers;

public class GameModel {
    public List<List<BoardCellModel>> Board = new List<List<BoardCellModel>>();

    public event EventHandler? StateChanged;
    public AutoProp<Faction> FactionTurn = new AutoProp<Faction>(Faction.Black);
    public AutoProp<PieceModel?> SelectedPiece = new AutoProp<PieceModel?>(null);

    public Location? SelectedLocation { get; private set; } = null;
    public List<Location>? MovableLocations { get; private set; } = null;

    public GameModel() {
        FactionTurn.ValueChanged += (s, v) => OnStateChanged();
        SelectedPiece.ValueChanged += (s, v) => {
            SelectedLocation = v == null ? null : FindPiece(v);
            MovableLocations = GetMovableLocations();
            OnStateChanged();
        };

        var nextPieceId = 0;

        for (int row = 0; row < Constants.BOARD_HEIGHT; row++) {
            var cellRow = new List<BoardCellModel>();

            for (int col = 0; col < Constants.BOARD_WIDTH; col++) {
                var cellModel = new BoardCellModel { Location = new Location(row, col) };
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

    public void MoveSelectedPiece(Location location) {
        if (SelectedLocation == null) {
            return;
        }

        var fromCell = Board[SelectedLocation.row][SelectedLocation.col];
        var toCell = Board[location.row][location.col];
        toCell.Piece = fromCell.Piece;
        fromCell.Piece = null;

        SelectedPiece.Value = null;
        FactionTurn.Value = FactionTurn.Value == Faction.Red ? Faction.Black : Faction.Red;
    }

    private void OnStateChanged() {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private Location FindPiece(PieceModel piece) {
        for (int row = 0; row < Board.Count; row++) {
            for (int col = 0; col < Board[0].Count; col++) {
                if (Board[row][col].Piece == piece) {
                    return new Location(row, col);
                }
            }
        }

        throw new Exception("Piece not found on board.");
    }

    private List<Location>? GetMovableLocations() {
        var locations = new List<Location>();
        if (SelectedLocation == null) {
            return null;
        }

        if (SelectedLocation.row > 0 && SelectedLocation.col > 0) {
            locations.Add(new Location(SelectedLocation.row - 1, SelectedLocation.col - 1));
        }

        if (SelectedLocation.row < Board.Count - 1 && SelectedLocation.col < Board[0].Count - 1) {
            locations.Add(new Location(SelectedLocation.row + 1, SelectedLocation.col + 1));
        }

        if (SelectedLocation.row > 0 && SelectedLocation.col < Board[0].Count - 1) {
            locations.Add(new Location(SelectedLocation.row - 1, SelectedLocation.col + 1));
        }

        if (SelectedLocation.row < Board.Count - 1 && SelectedLocation.col > 0) {
            locations.Add(new Location(SelectedLocation.row + 1, SelectedLocation.col - 1));
        }

        return locations;
    }
}
