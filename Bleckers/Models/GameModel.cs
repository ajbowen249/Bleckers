namespace Bleckers;

public class GameModel {
	public event EventHandler? StateChanged;
	public AutoProp<Faction> FactionTurn = new AutoProp<Faction>(Faction.Black);

	public GameModel() {
		FactionTurn.ValueChanged += (s, e) => OnStateChanged();
	}

	private void OnStateChanged() {
		StateChanged?.Invoke(this, EventArgs.Empty);
	}
}
