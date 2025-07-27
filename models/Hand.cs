using PokerSim.Enums;

namespace PokerSim.models;

public class Hand(HandRank handIndex, int cardCount = 0)
{
    public HandRank HandIndex = handIndex;
    public int Count = cardCount;

    public void IncrementCount()
    {
        Count++;
    }

    public override string ToString()
    {
        return $"{HandIndex.ToString()}: {Count}";
    }

    public string GetName()
    {
        return HandIndex.ToString();
    }

    public int GetCount()
    {
        return Count;
    }

    public Hand Clone()
    {
        return new Hand(HandIndex);
    }
}