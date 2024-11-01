using System.Collections.Immutable;

namespace Bleckers;

public class MovementOption {
    public required Location Location { get; set; }
    public BoardCellModel? CaptureCell { get; set; }
}

public class GameModel {
    public List<List<BoardCellModel>> Board = new List<List<BoardCellModel>>();

    public event EventHandler? StateChanged;
    public AutoProp<Faction> FactionTurn = new AutoProp<Faction>(Faction.Black);
    public AutoProp<PieceModel?> SelectedPiece = new AutoProp<PieceModel?>(null);
    public AutoProp<ImmutableDictionary<Faction, int>> FactionPieceCounts = new AutoProp<ImmutableDictionary<Faction, int>>(
        new Dictionary<Faction, int> {
            { Faction.Black, 0 },
            { Faction.Red, 0 },
        }.ToImmutableDictionary()
    );

    public Location? SelectedLocation { get; private set; } = null;
    public List<MovementOption>? MovableLocations { get; private set; } = null;

    public GameModel() {
        FactionTurn.ValueChanged += (s, v) => OnStateChanged();
        FactionPieceCounts.ValueChanged += (s, v) => OnStateChanged();
        SelectedPiece.ValueChanged += (s, v) => UpdateSelectedPiece(v);

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
                    };

                    cellModel.Piece = piece;
                }

                cellRow.Add(cellModel);
            }

            Board.Add(cellRow);
        }
    }

    public void MoveSelectedPiece(MovementOption option) {
        if (SelectedLocation == null || SelectedPiece.Value == null) {
            return;
        }

        var fromCell = Board[SelectedLocation.row][SelectedLocation.col];
        var toCell = Board[option.Location.row][option.Location.col];
        toCell.Piece = fromCell.Piece;
        fromCell.Piece = null;

        if (option.CaptureCell != null) {
            option.CaptureCell.Piece = null;
            var pieceCount = FactionPieceCounts.Value[FactionTurn.Value] + 1;
            FactionPieceCounts.Value = FactionPieceCounts.Value.SetItem(FactionTurn.Value, pieceCount);
        }

        UpdateSelectedPiece(SelectedPiece.Value);

        if (!SelectedPiece.Value.IsKing) {
            if (
                (SelectedPiece.Value.Faction == Faction.Red && option.Location.row == 0) ||
                (SelectedPiece.Value.Faction == Faction.Black && option.Location.row == Constants.BOARD_HEIGHT - 1)
             ) {
                SelectedPiece.Value.IsKing = true;
            }
        }

        UpdateSelectedPiece(SelectedPiece.Value);

        // If we just captured a cell and can capture another, don't switch the turn
        if (option.CaptureCell == null || MovableLocations == null || !MovableLocations.Any(x => x.CaptureCell != null)) {
            SelectedPiece.Value = null;
            FactionTurn.Value = FactionTurn.Value == Faction.Red ? Faction.Black : Faction.Red;
        }
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

    private List<MovementOption>? GetMovableLocations() {
        if (SelectedLocation == null || SelectedPiece.Value == null) {
            return null;
        }

        var allDeltas = new List<Location> {
            new Location(-1, -1),
            new Location(-1,  1),
            new Location( 1, -1),
            new Location( 1,  1),
        };

        var deltas = new List<Location>();

        if (SelectedPiece.Value.IsKing) {
            deltas = allDeltas;
        } else {
            if (SelectedPiece.Value.Faction == Faction.Red) {
                deltas = new List<Location> { allDeltas[0], allDeltas[1] };
            } else {
                deltas = new List<Location> { allDeltas[2], allDeltas[3] };
            }
        }

        var options = new List<MovementOption>();

        foreach (var delta in deltas) {
            var newLocation = SelectedLocation + delta;
            if (!newLocation.IsInBoard()) {
                continue;
            }

            var landingCell = GetLocation(newLocation);
            if (landingCell.Piece == null) {
                options.Add(new MovementOption { Location = newLocation });
            } else if (landingCell.Piece.Faction != SelectedPiece.Value.Faction) {
                // Can only capture if the landing zone is clear.
                newLocation += delta;
                if (newLocation.IsInBoard() && GetLocation(newLocation).Piece == null) {
                    options.Add(new MovementOption {
                        Location = newLocation,
                        CaptureCell = landingCell,
                    });
                }
            }
        }

        return options;
    }

    private BoardCellModel GetLocation(Location location) {
        return Board[location.row][location.col];
    }

    private void UpdateSelectedPiece(PieceModel? piece) {
        SelectedLocation = piece == null ? null : FindPiece(piece);
        MovableLocations = GetMovableLocations();
        OnStateChanged();
    }
}
