using PokerSim.models;

namespace PokerSim.Logic;

public static class Printer
{
    public static void PrintCards(Card[] cards)
    {
        foreach (Card card in cards)
        {
            Console.WriteLine(card.ToString());
        }
    }

    public static string GetHandSummary(Hand[] hands)
    {
        string results = "";
        foreach (Hand hand in hands)
        {
            results += " " + hand.ToString() + "\n";
        }

        return results.Trim();
    }

    public static string GetHandSummaryTotal(Hand[] hands, int totalHands)
    {
        string results = "";
        foreach (Hand hand in hands)
        {
            float ratio = (float)hand.GetCount() / (float)totalHands;
            results += $" {hand.GetName(),-13}: {MathF.Round(ratio * 100, 3)}% ({hand.GetCount()})\n";
        }

        return results.Trim();
    }

    public static string GetHandSummaryWinning(Hand[] winningHands, int totalRounds)
    {
        string results = "";
        foreach (Hand hand in winningHands)
        {
            float ratio = (float)hand.GetCount() / totalRounds;
            results += $" {hand.GetName(),-13}: {MathF.Round(ratio * 100, 3)}% ({hand.GetCount()})\n";
        }

        return results.Trim();
    }


    public static string GetHandWinEfficiency(Hand[] winningHands, Hand[] totalHands)
    {
        string results = "";
        for (int i = 0; i < winningHands.Length; i++)
        {
            var total = totalHands[i].GetCount();
            var wins = winningHands[i].GetCount();

            if (total == 0)
                results += $" {winningHands[i].GetName(),-13}: --- (0 occurrences)\n";
            else
            {
                float ratio = (float)wins / total;
                results += $" {winningHands[i].GetName(),-13}: {MathF.Round(ratio * 100, 3)}% ({wins}/{total})\n";
            }
        }

        return results.Trim();
    }
}