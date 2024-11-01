namespace Bleckers;
public class PieceModel {
    public required Faction Faction { get; set; }
    public required int ID { get; set; }
    public bool IsKing { get; set; } = false;
}
