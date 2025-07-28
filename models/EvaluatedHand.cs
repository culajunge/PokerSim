using PokerSim.Enums;

namespace PokerSim.models;

public class EvaluatedHand : IComparable<EvaluatedHand>
{
    public HandRank Rank { get; }
    public Card[] BestFiveCards { get; }
    public Card[] Kickers { get; }

    public EvaluatedHand(HandRank rank, Card[] bestFiveCards, Card[] kickers)
    {
        Rank = rank;
        BestFiveCards = bestFiveCards;
        Kickers = kickers;
    }

    public int CompareTo(EvaluatedHand other)
    {
        if (Rank != other.Rank)
            return Rank.CompareTo(other.Rank);

        for (int i = 0; i < BestFiveCards.Length; i++)
        {
            int cmp = BestFiveCards[i].Rank.CompareTo(other.BestFiveCards[i].Rank);
            if (cmp != 0) return cmp;
        }

        for (int i = 0; i < Math.Min(Kickers.Length, other.Kickers.Length); i++)
        {
            int cmp = Kickers[i].Rank.CompareTo(other.Kickers[i].Rank);
            if (cmp != 0) return cmp;
        }

        return 0;
    }
}
