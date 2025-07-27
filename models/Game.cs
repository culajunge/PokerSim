using System.Diagnostics;
using System.Runtime.CompilerServices;
using PokerSim.Enums;
using PokerSim.Logic;
using System.Linq;

namespace PokerSim.models;

public class Game(
    int players,
    int holeCards,
    int communityCards,
    int gameIndex = 0,
    bool shuffleDeck = true)
{
    public int PlayerAmount = players;
    public int HoleCardAmount = holeCards;
    public int CommunityCardAmount = communityCards;
    private bool _gameInitialized = false;
    public int RoundsPlayed = 0;
    public int GameIndex = 0;

    public Hand[] Hands;
    public Hand[] BestHands;
    private Deck deck;
    private Card[] communityCards;

    public void PlayGame(int rounds)
    {
        for (int i = 0; i < rounds; i++)
        {
            PlayRound();
            Interlocked.Increment(ref Program._globalRoundsPlayed);
            RoundsPlayed++;
        }
    }

    public void PlayRound()
    {
        if (!_gameInitialized)
        {
            Console.WriteLine("WARNING: Game is not initialized, auto initialized");
            InitializeGame();
        }

        communityCards = deck.Draw(CommunityCardAmount);
        List<EvaluatedHand> evaluatedHands = new List<EvaluatedHand>();
        HandRank highestRank = HandRank.HighCard;

        if (communityCards == null)
            throw new InvalidOperationException("Community cards are missing.");


        // Evaluate each player's hand and track stats
        for (int i = 0; i < PlayerAmount; i++)
        {
            EvaluatedHand evaluated = PlayHand(); // Now returns EvaluatedHand
            evaluatedHands.Add(evaluated);
            Hands[(int)evaluated.Rank].IncrementCount();

            if ((int)evaluated.Rank > (int)highestRank)
            {
                highestRank = evaluated.Rank;
            }
        }

        // Filter all players that had the highest rank
        var contenders = evaluatedHands
            .Where(eh => eh.Rank == highestRank)
            .ToList();

        // If there's a tie, compare bestFiveCards for actual winner(s)
        EvaluatedHand bestHand = contenders[0];
        if (bestHand == null || bestHand.BestFiveCards == null)
            throw new InvalidOperationException("The best cards are missing.");
        foreach (var contender in contenders.Skip(1))
        {
            int cmp = CompareHands(bestHand.BestFiveCards, contender.BestFiveCards);
            if (cmp < 0) bestHand = contender; // contender is better
        }

        // Record the final winning hand
        BestHands[(int)bestHand.Rank].IncrementCount();
    }

    public static int CompareHands(Card[] a, Card[] b)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        // Sort both hands high to low
        var sortedA = a.OrderByDescending(card => (int)card.Rank).ToArray();
        var sortedB = b.OrderByDescending(card => (int)card.Rank).ToArray();

        for (int i = 0; i < 5; i++)
        {
            int diff = (int)sortedA[i].Rank - (int)sortedB[i].Rank;
            if (diff != 0) return diff;
        }

        return 0; // Tie
    }

    EvaluatedHand PlayHand()
    {
        return PokerHandEvaluator.EvaluateBestHand([..deck.Draw(HoleCardAmount), ..communityCards]);
    }

    public int GetRoundsPlayed()
    {
        return RoundsPlayed;
    }

    public Deck GetDeck()
    {
        return deck;
    }

    public void InitializeGame()
    {
        InitializeHands();

        deck = new Deck();
        deck.Fill();
        if (shuffleDeck) deck.Shuffle();

        _gameInitialized = true;
    }

    void InitializeHands()
    {
        Hand[] _hands = new Hand[10];
        int index = 0;
        foreach (HandRank handRank in Enum.GetValues(typeof(HandRank)))
        {
            _hands[index] = new Hand(handRank);
            index++;
        }

        Hands = _hands;
        BestHands = Hands.Select(h => h.Clone()).ToArray();
    }

    public string DbgGetHands()
    {
        string result = "";
        foreach (Hand hand in Hands)
        {
            result += hand.ToString() + "\n";
        }

        return result;
    }

    public override string ToString()
    {
        return
            $"Game {gameIndex} \nPlayers: {PlayerAmount}\nHoleCards: {HoleCardAmount}, CommunityCards: {CommunityCardAmount}\n ___ \n \nRoundsPlayed: {RoundsPlayed}\n \nBestHands (Wins): \n{Printer.GetHandSummary(BestHands)}\nHands (total): \n{Printer.GetHandSummary(Hands)}";
    }
}