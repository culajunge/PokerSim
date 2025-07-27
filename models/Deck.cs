using PokerSim.Enums;

namespace PokerSim.models;

public class Deck
{
    public Card[] Cards { get; set; } = null!;
    public int GetCardCount() => Cards.Length;
    private int drawIndex = 0;

    public Card[] Draw(int cardAmount)
    {
        EnsureEnoughCards(cardAmount);

        Card[] result = new Card[cardAmount];
        for (int i = 0; i < cardAmount; i++)
        {
            result[i] = Cards[i + drawIndex];
        }

        drawIndex += cardAmount;
        return result;
    }

    void EnsureEnoughCards(int pulledCardAmount = 0)
    {
        if (drawIndex + pulledCardAmount > Cards.Length)
        {
            drawIndex = 0;
            Shuffle();
        }
    }

    public void Fill()
    {
        var cards = new List<Card>();

        foreach (Suits suit in Enum.GetValues(typeof(Suits)))
        {
            foreach (Ranks rank in Enum.GetValues(typeof(Ranks)))
            {
                cards.Add(new Card(suit, rank));
            }
        }

        Cards = cards.ToArray();
    }

    public void Shuffle()
    {
        Random rng = new();
        Cards = Cards.OrderBy(_ => rng.Next()).ToArray(); // Fisher-Yates is more performant for large arrays
    }

    public override string ToString()
    {
        string result = string.Empty;
        foreach (Card card in Cards)
        {
            result += $"{card.ToString()}\n";
        }

        return result;
    }
}