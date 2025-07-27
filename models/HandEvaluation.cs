using PokerSim.Enums;

namespace PokerSim.models;

public class HandEvaluation : IComparable<HandEvaluation>
{
    public HandRank Rank { get; set; }
    public List<int> TieBreakers { get; set; } // e.g. [14, 13, 11, 9, 6] for A-high

    public int CompareTo(HandEvaluation other)
    {
        if (Rank != other.Rank)
            return Rank.CompareTo(other.Rank);

        for (int i = 0; i < Math.Min(TieBreakers.Count, other.TieBreakers.Count); i++)
        {
            if (TieBreakers[i] != other.TieBreakers[i])
                return TieBreakers[i].CompareTo(other.TieBreakers[i]);
        }

        return 0; // Exact tie
    }

    public override string ToString() =>
        $"{Rank} ({string.Join(", ", TieBreakers.Select(r => r.ToString()))})";
}