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

        //Draw community cards 
        communityCards = deck.Draw(CommunityCardAmount);
        HandRank highestHand = HandRank.HighCard;

        for (int i = 0; i < PlayerAmount; i++)
        {
            HandRank hand = PlayHand();
            Hands[(int)hand].IncrementCount();
            if ((int)hand > (int)highestHand)
            {
                highestHand = hand;
            }
        }

        BestHands[(int)highestHand].IncrementCount();
    }

    HandRank PlayHand()
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