public class MatchSettings {

    public bool FriendlyFire { get; set; }
	public double RoundTime { get; set; }
	public int RoundsInMatch { get; set; }
    public int WarmupTime { get; set; }
    public int EndOfRoundTime { get; set; }

    private static MatchSettings _instance;

    private MatchSettings() {
        this.FriendlyFire = false;
		this.RoundTime = 5 * 60; // 5 min ( 5 * 60 seconds)
        this.RoundsInMatch = 7;
        this.WarmupTime = 10;
        this.EndOfRoundTime = 5;
    }

    public static MatchSettings Instance {
        get {
            if (_instance == null) {
                _instance = new MatchSettings();
            }
            return _instance;
        }
    }
}