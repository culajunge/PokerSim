using PokerSim.models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Text;
using PokerSim.Dto;

namespace PokerSim.Logic;

public static class Printer
{
    public static bool UseTooltips = false;

    public static void PrintCards(Card[] cards)
    {
        foreach (Card card in cards)
        {
            Console.WriteLine(card.ToString());
        }
    }

    public static void PrintToolTip(string input)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(input);
        Console.ResetColor();
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
            results +=
                $"{hand.GetName(),-13} {RenderBar(ratio)} {MathF.Round(ratio * 100, 3)}% ({hand.GetCount()})\n";
        }

        return results.Trim();
    }

    public static string GetHandSummaryWinning(Hand[] winningHands, int totalRounds)
    {
        string results = "";
        foreach (Hand hand in winningHands)
        {
            float ratio = (float)hand.GetCount() / totalRounds;
            string tooltip = GetWinningTooltip(ratio);
            results +=
                $"{hand.GetName(),-13} {RenderBar(ratio)} {MathF.Round(ratio * 100, 3)}% ({hand.GetCount()}) - {tooltip,-13}\n";
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
                results += $"{winningHands[i].GetName(),-13}: --- (0 occurrences)\n";
            else
            {
                float ratio = (float)wins / total;
                string tooltip = GetEfficiencyTooltip(wins, total);
                results +=
                    $"{winningHands[i].GetName(),-13} {RenderBar(ratio)} {MathF.Round(ratio * 100, 3)}% ({wins}/{total})  {tooltip}\n";
            }
        }

        return results.Trim();
    }

    public static string GetOverperformance(Hand[] winningHands, Hand[] totalHands)
    {
        var totalHandCount = totalHands.Sum(h => h.Count);
        var totalWinCount = winningHands.Sum(h => h.Count);

        var builder = new StringBuilder();
        builder.AppendLine("\n📊 Overperformance:");
        builder.AppendLine(new string('-', 50));
        builder.AppendLine($"{"Hand",-13} {"Occ%",6} {"Win%",6} {"Luck"}   Chart");
        builder.AppendLine(new string('-', 50));

        foreach (var hand in totalHands.OrderByDescending(h => h.Count))
        {
            var total = hand.Count;
            var wins = winningHands.FirstOrDefault(w => w.HandIndex == hand.HandIndex)?.Count ?? 0;

            float occRatio = (float)total / totalHandCount;
            float winRatio = totalWinCount > 0 ? (float)wins / totalWinCount : 0;
            float luckFactor = occRatio > 0 ? winRatio / occRatio : 0;

            string bar = new string(luckFactor switch
            {
                >= 2f => '+',
                >= 1.5f => '=',
                >= 1.1f => '-',
                >= 0.9f => '.',
                >= 0.5f => '~',
                > 0f => ' ',
                _ => ' '
            }, (int)(luckFactor * 2));

            string formatted = $"{hand.HandIndex,-13} {occRatio,5:P0} {winRatio,5:P0}  {luckFactor,5:0.0}   {bar}";
            builder.AppendLine(formatted);
        }

        PrintToolTip("Values greater than one considered lucky :), values less than one considered unlucky :(");
        return builder.ToString();
    }

    public static string GetWinningTooltip(float ratio)
    {
        if (!UseTooltips) return "";
        if (ratio > 0.9f)
            return "Dominates the game.";
        else if (ratio > 0.5f)
            return "Often the winning hand.";
        else if (ratio > 0.2f)
            return "Common winner.";
        else if (ratio > 0f)
            return "Occasionally wins.";
        else
            return "Never won in the simulation.";
    }

    public static string GetEfficiencyTooltip(int wins, int total)
    {
        if (!UseTooltips) return "";
        if (total == 0)
            return "Never occurred in simulation.";

        float ratio = (float)wins / total;

        if (ratio > 0.9f)
            return "Wins almost every time it appears.";
        else if (ratio > 0.75f)
            return "Wins most of the time it appears.";
        else if (ratio > 0.5f)
            return "Wins more than half the time it appears.";
        else if (ratio > 0.25f)
            return "Wins occasionally.";
        else if (ratio > 0f)
            return "Rarely wins when it appears.";
        else
            return "Never wins despite appearing.";
    }

    public static void PrintWinShare(SimulationStatsDto stats)
    {
        Console.WriteLine("\n🎯 Win Share (% of total wins by hand):");
        Console.WriteLine(new string('-', 40));

        int totalWins = stats.WinningHands.Sum(h => h.Count);

        int rank = 1;
        foreach (var hand in stats.WinningHands.OrderByDescending(h => h.Count))
        {
            double share = (double)hand.Count / totalWins * 100;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{$"{rank}. " + hand.Name,-20} {share,6:F2} %");
            rank++;
        }

        Console.ResetColor();
    }

    public static void PrintSummary(SimulationStatsDto stats)
    {
        Console.WriteLine("\n📊 Summary:");
        Console.WriteLine(new string('-', 40));

        // Average Hand (most dealt)
        var avgHand = stats.TotalHands.OrderByDescending(h => h.Count).FirstOrDefault();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Average hand: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{avgHand?.Name} ({avgHand?.Count} times)");

        // Average Winning Hand (most won)
        var avgWinHand = stats.WinningHands.OrderByDescending(h => h.Count).FirstOrDefault();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Average winning hand: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{avgWinHand?.Name} ({avgWinHand?.Count} wins)");

        // Safe Advantage Hand (first with win ratio ≥ 50%)
        var totalLookup = stats.TotalHands.ToDictionary(h => h.Name, h => h.Count);

        string[] handOrder = new[]
        {
            "HighCard", "OnePair", "TwoPair", "ThreeOfAKind",
            "Straight", "Flush", "FullHouse", "FourOfAKind", "StraightFlush", "RoyalFlush"
        };

        var safeHand = handOrder
            .Where(name => totalLookup.TryGetValue(name, out var total) && total > 0)
            .Select(name =>
            {
                var wins = stats.WinningHands.FirstOrDefault(h => h.Name == name)?.Count ?? 0;
                double winRatio = (double)wins / totalLookup[name];
                return new { Name = name, WinRatio = winRatio };
            })
            .FirstOrDefault(x => x.WinRatio >= 0.5);

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Safe advantage hand (≥50% win ratio): ");
        if (safeHand != null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{safeHand.Name} ({safeHand.WinRatio:P2})");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("None found");
        }

        Console.ResetColor();
    }

    public static string RenderBar(float percentage, int width = 30)
    {
        int filled = Math.Min((int)(percentage * width), width);
        string bar = new string('█', filled) + new string('░', width - filled);
        return $"|{bar}|";
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

    public static void PrintFromJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"❌ File not found: {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);

        // Deserialize into DTO
        var dto = JsonSerializer.Deserialize<SimulationStatsDto>(json);

        if (dto is null)
        {
            Console.WriteLine("❌ Failed to parse JSON.");
            return;
        }

        // Map DTO to your SimulationStats domain object
        var totalHands = dto.TotalHands.Select(HandMapper.Map).ToList();
        var winningHands = dto.WinningHands.Select(HandMapper.Map).ToList();

        Console.WriteLine("\u2665\ufe0f Poker Simulation Results\n");
        Console.WriteLine(
            $"Players: {dto.Players}\nHoleCards: {dto.HoleCards}\nCommunityCards: {dto.CommunityCards}\nRounds played: {dto.RoundsPlayed}\nHands played: {dto.RoundsPlayed * dto.Players}\nCards played: {dto.RoundsPlayed * (dto.Players * dto.HoleCards + dto.CommunityCards)}\nCheers\n");

        // Use your existing logic
        Console.WriteLine("🃏 Hand Occurrences:");
        PrintToolTip("Amount of specific Hand out of all Hands");
        Console.WriteLine(GetHandSummaryTotal(totalHands.ToArray(), dto.RoundsPlayed * dto.Players));

        Console.WriteLine("\n🏆 Hand Win Rates:");
        PrintToolTip("Amount of Wins gathered by specific Hand");
        Console.WriteLine(GetHandSummaryWinning(winningHands.ToArray(), dto.RoundsPlayed));

        Console.WriteLine("\n⚖️  Win Efficiency:");
        PrintToolTip("Amount of Wins gathered by specific Hand out of all those specific Hand");
        Console.WriteLine(GetHandWinEfficiency(winningHands.ToArray(), totalHands.ToArray()));

        PrintWinShare(dto);

        Console.WriteLine(GetOverperformance(winningHands.ToArray(), totalHands.ToArray()));

        PrintSummary(dto);
    }
}