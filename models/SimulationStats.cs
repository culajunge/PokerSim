namespace PokerSim.models;

public class SimulationStats
{
    public int Players { get; set; }
    public int HoleCards { get; set; }
    public int CommunityCards { get; set; }

    public int RoundsPlayed { get; set; }

    public Hand[] TotalHands { get; set; }
    public Hand[] WinningHands { get; set; }
}