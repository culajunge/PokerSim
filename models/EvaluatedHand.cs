using PokerSim.Enums;

namespace PokerSim.models;

public class EvaluatedHand
{
    public HandRank Rank { get; }
    public Card[] BestFiveCards { get; }

    public EvaluatedHand(HandRank rank, Card[] bestFiveCards)
    {
        Rank = rank;
        BestFiveCards = bestFiveCards;
    }

    public override string ToString()
    {
        return $"{Rank}";
    }
}