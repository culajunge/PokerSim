using PokerSim.Enums;

namespace PokerSim.models;

public class Card(Suits suit, Ranks rank)
{
    public Suits Suit = suit;
    public Ranks Rank = rank;

    public override string ToString() => $"{Rank} of {Suit}";
}