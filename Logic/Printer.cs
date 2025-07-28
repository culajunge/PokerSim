using PokerSim.models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

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

    public static void SaveResultsToJson(
        Hand[] totalHands,
        Hand[] totalBestHands,
        int players,
        int holeCards,
        int communityCards,
        int rounds
    )
    {
        var results = new
        {
            Players = players,
            HoleCards = holeCards,
            CommunityCards = communityCards,
            RoundsPlayed = rounds,
            TotalHands = totalHands.Select(h => new { Name = h.HandIndex.ToString(), Count = h.Count }),
            WinningHands = totalBestHands.Select(h => new { Name = h.HandIndex.ToString(), Count = h.Count })
        };

        // Build directory path: ./2HoleCards/5CommCards/6Player/
        string dirPath = Path.Combine(
            "Results",
            $"{holeCards}HoleCards",
            $"{communityCards}CommCards",
            $"{players}Player"
        );

        Directory.CreateDirectory(dirPath);

        // Base filename
        string baseFilename = $"PokerSim_P{players}_H{holeCards}_C{communityCards}_R{rounds}.json";
        string filePath = Path.Combine(dirPath, baseFilename);

        // Avoid overwriting: Add suffix like _2, _3, etc.
        int suffix = 2;
        while (File.Exists(filePath))
        {
            string fileNameWithSuffix = Path.GetFileNameWithoutExtension(baseFilename) + $"_{suffix}" + ".json";
            filePath = Path.Combine(dirPath, fileNameWithSuffix);
            suffix++;
        }

        // Serialize and save
        string json = JsonSerializer.Serialize(results, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filePath, json);
        Console.WriteLine($"📁 Results saved to {filePath}");
    }
}