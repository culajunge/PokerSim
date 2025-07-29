using System.Diagnostics;
using PokerSim.Enums;
using PokerSim.Logic;
using PokerSim.models;

namespace PokerSim;

class Program
{
    private static (int start, int end) PlayerRange = (6, 6);
    private static (int start, int end) HoleCardRange = (2, 2);
    private static (int start, int end) CommunityCardRange = (5, 5);

    private static int Players = 6;
    private static int HoleCards = 2;
    private static int CommunityCards = 5;


    private static int RoundsPerGame = 10;
    private static int TotalGames = 5;


    static bool _running = true;

    static List<Game> _games = new List<Game>();

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "♠ P0K3R Game Simulator ♣";
        Console.Clear();
        WriteLineWithColor("🃏 P0K3R Game Simulator", ConsoleColor.Magenta);
        Thread.Sleep(100);

        string testPath = Directory.GetCurrentDirectory(); // or wherever you're writing files

        if (!HasWriteAccess(testPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Write permission denied for directory: {testPath}");
            Console.WriteLine(
                "Please run this application with write permissions (e.g., use `chmod +w` or run from a different location).");
            Console.ResetColor();
            Environment.Exit(1);
        }
        else
        {
            Console.WriteLine($"✅ Write permission OK in: {testPath}");
        }

        while (_running)
        {
            Console.WriteLine("\n📋 Main Menu:");
            Console.WriteLine("1. 🔧 Run Simulation");
            Console.WriteLine("2. 📂 View Results");
            Console.Write("Select an option (1/2): ");
            string? choice = Console.ReadLine();

            if (choice == "2")
            {
                ViewResultsMenu();
                continue;
            }

            Console.WriteLine("\n🔧 Configure Simulation:\n");
            PrintConfigMenu();

            Console.Write("\nStart Simulation?");
            if (!GetBool()) continue;

            SetupCtrlCHandler();

            Console.WriteLine("\n⏳ Running simulation...\n");
            for (int players = PlayerRange.start; players <= PlayerRange.end; players++)
            {
                for (int holeCards = HoleCardRange.start; holeCards <= HoleCardRange.end; holeCards++)
                {
                    for (int commCards = CommunityCardRange.start; commCards <= CommunityCardRange.end; commCards++)
                    {
                        Console.WriteLine($"\n⚙️  Simulating P:{players}, H:{holeCards}, C:{commCards}");

                        Players = players;
                        HoleCards = holeCards;
                        CommunityCards = commCards;

                        _globalRoundsPlayed = 0;
                        var stopwatch = Stopwatch.StartNew();

                        var cts = new CancellationTokenSource();
                        var loadingTask = Task.Run(() => ShowLoadingAnimation(cts.Token));

                        var totalRoundsToPlay = TotalGames * RoundsPerGame;
                        var progressCts = new CancellationTokenSource();
                        RenderProgressBar(0, 0, 0, TimeSpan.Zero);
                        var progressTask = Task.Run(() =>
                        {
                            while (!progressCts.Token.IsCancellationRequested)
                            {
                                double progress = (double)_globalRoundsPlayed / totalRoundsToPlay;
                                RenderProgressBar(progress, _globalRoundsPlayed, totalRoundsToPlay, stopwatch.Elapsed);
                                Thread.Sleep(100);
                            }
                        });

                        var completedGames = StartGamesAsync(TotalGames).GetAwaiter().GetResult();
                        _games = completedGames;

                        progressCts.Cancel();
                        progressTask.Wait();
                        cts.Cancel();
                        loadingTask.Wait();
                        Console.WriteLine();

                        var (totalHands, totalBestHands, totalRounds) = SummarizeGames(_games);

                        WriteLineWithColor("📊 Results", ConsoleColor.Green);
                        P($"Total Rounds Played: {totalRounds}");
                        WriteLineWithColor("\n👐 Hand Occurrences:", ConsoleColor.Yellow);
                        int totalHandsPlayed = players * totalRounds;
                        P(Printer.GetHandSummaryTotal(totalHands.ToArray(), totalHandsPlayed));

                        WriteLineWithColor("\n🏆 Winning Hands Summary:", ConsoleColor.Cyan);
                        P(Printer.GetHandSummaryWinning(totalBestHands.ToArray(), totalRounds));

                        WriteLineWithColor("\n📈 Win Efficiency (per hand type):", ConsoleColor.Magenta);
                        P(Printer.GetHandWinEfficiency(totalBestHands.ToArray(), totalHands.ToArray()));
                        P("\n✅ Simulation Complete!\n");

                        Printer.SaveResultsToJson(
                            totalHands.ToArray(),
                            totalBestHands.ToArray(),
                            players,
                            holeCards,
                            commCards,
                            totalRounds
                        );

                        _games.Clear(); // Prepare for next combo
                    }
                }
            }
        }
    }

    static void ViewResultsMenu()
    {
        string resultsRoot = Path.Combine(Directory.GetCurrentDirectory(), "Results");

        if (!Directory.Exists(resultsRoot))
        {
            WriteLineWithColor("❌ Results folder not found.", ConsoleColor.Red);
            return;
        }

        string? holeFolder = SelectOptionFromDirectory(resultsRoot, "🎴 Hole Card Options");
        if (holeFolder == null) return;

        string holePath = Path.Combine(resultsRoot, holeFolder);
        string? commFolder = SelectOptionFromDirectory(holePath, "🃏 Community Card Options");
        if (commFolder == null) return;

        string commPath = Path.Combine(holePath, commFolder);
        string? playerFolder = SelectOptionFromDirectory(commPath, "👥 Player Count Options");
        if (playerFolder == null) return;

        string playerPath = Path.Combine(commPath, playerFolder);
        string[] jsonFiles = Directory.GetFiles(playerPath, "*.json");

        if (jsonFiles.Length == 0)
        {
            WriteLineWithColor("⚠ No result files found.", ConsoleColor.Yellow);
            return;
        }

        Console.WriteLine("\n📄 Available Result Files:");
        for (int i = 0; i < jsonFiles.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {Path.GetFileName(jsonFiles[i])}");
        }

        Console.Write("Select file to view: ");
        if (int.TryParse(Console.ReadLine(), out int fileChoice) &&
            fileChoice >= 1 && fileChoice <= jsonFiles.Length)
        {
            string filePath = jsonFiles[fileChoice - 1];
            Console.WriteLine(filePath + "\n");
            Printer.PrintFromJson(filePath);
            Console.ResetColor();
        }
        else
        {
            WriteLineWithColor("❌ Invalid file choice.", ConsoleColor.Red);
        }
    }

    static string? SelectOptionFromDirectory(string path, string label)
    {
        var dirs = Directory.GetDirectories(path)
            .Select(d => Path.GetFileName(d))
            .ToList();

        if (dirs.Count == 0)
        {
            WriteLineWithColor($"⚠ No options found in {label}.", ConsoleColor.Yellow);
            return null;
        }

        Console.WriteLine($"\n{label}:");
        foreach (var dir in dirs)
        {
            Console.WriteLine($"- {dir}");
        }

        Console.Write("Select option (by number from folder name): ");
        string? input = Console.ReadLine();

        if (int.TryParse(input, out int number))
        {
            // Match any folder that starts with that number (e.g., 5 -> "5CommCards")
            string? match = dirs.FirstOrDefault(d => d.StartsWith(number.ToString()));
            if (match != null)
                return match;
        }

        WriteLineWithColor("❌ Invalid selection.", ConsoleColor.Red);
        return null;
    }


    public static int _globalRoundsPlayed = 0;

    static async Task<List<Game>> StartGamesAsync(int amount)
    {
        var gameTasks = new List<Task<Game>>();

        for (int i = 0; i < amount; i++)
        {
            int gameIndex = i;
            gameTasks.Add(Task.Run(() =>
            {
                var game = new Game(Players, HoleCards, CommunityCards, gameIndex);
                game.InitializeGame();
                game.PlayGame(RoundsPerGame);
                return game;
            }));
        }

        var finishedGames = await Task.WhenAll(gameTasks);
        return finishedGames.ToList();
    }

    public static (List<Hand> totalHands, List<Hand> summedBestHands, int totalRounds) SummarizeGames(List<Game> games)
    {
        const int HandTypes = 10;

        var totalHands = new List<Hand>(new Hand[HandTypes]);
        var totalBestHands = new List<Hand>(new Hand[HandTypes]);
        int totalRounds = 0;

        // Initialize empty hands of each type (assuming enum HandRank from 0–9)
        for (int i = 0; i < HandTypes; i++)
        {
            totalHands[i] = new Hand(((HandRank)i)); // e.g. HandRank.Pair, HandRank.TwoPair, etc.
            totalBestHands[i] = new Hand(((HandRank)i));
        }

        foreach (var game in games)
        {
            totalRounds += game.RoundsPlayed;

            // Sum regular hands
            if (game.Hands != null)
            {
                foreach (var hand in game.Hands)
                {
                    int index = (int)hand.HandIndex;
                    totalHands[index].Count += hand.Count;
                }
            }

            // Sum best hands (winners per round)
            if (game.BestHands != null)
            {
                foreach (var bestHand in game.BestHands)
                {
                    int index = (int)bestHand.HandIndex;
                    totalBestHands[index].Count += bestHand.Count;
                }
            }
        }

        return (totalHands, totalBestHands, totalRounds);
    }

    static void ClearAndBanner()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        P("♠♥♦♣ P0K3R Game Simulator ♠♥♦♣");
        Console.ResetColor();
    }

    static void PrintConfigMenu()
    {
        P("🎛️ Configuration (enter a single number or range like 3-6)");

        PlayerRange = AskRange("👥 Number of Players", PlayerRange.start);
        HoleCardRange = AskRange("🎴 Hole Cards per Player", HoleCardRange.start);
        CommunityCardRange = AskRange("🃏 Community Cards", CommunityCardRange.start);

        RoundsPerGame = AskInt("🔁 Rounds per Game", RoundsPerGame);
        TotalGames = AskInt("♻️ Total Games to Simulate", TotalGames);

        Console.WriteLine("\n🧩 Final Configuration:");
        Console.ForegroundColor = ConsoleColor.Yellow;
        PrintConfig();
        Console.ResetColor();
    }

    static void RenderProgressBar(double progress, int currentRound, int totalRounds, TimeSpan elapsed)
    {
        const int barWidth = 50;

        // Clamp progress between 0 and 1
        progress = Math.Max(0, Math.Min(1, progress));
        int filled = (int)(barWidth * progress);
        int remaining = barWidth - filled;

        string bar = $"[{new string('#', filled)}{new string('-', remaining)}]";

        // Time estimation
        string eta = "";
        if (progress > 0)
        {
            var estimatedTotal = TimeSpan.FromTicks((long)(elapsed.Ticks / progress));
            var timeLeft = estimatedTotal - elapsed;
            double roundsPerSecond = currentRound / elapsed.TotalSeconds;

            eta = $" | ETA: {timeLeft:mm\\:ss} | Speed: {roundsPerSecond:0.0} rps";
        }

        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write($"{bar} {currentRound}/{totalRounds}{eta}  ");
    }


    static void ShowLoadingAnimation(CancellationToken token)
    {
        /*
            string[] suits = { "♠", "♥", "♦", "♣" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            var rand = new Random();
            int width = Console.WindowWidth - 2;

            while (!token.IsCancellationRequested)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                string line = "";
                for (int i = 0; i < width / 6; i++)
                {
                    string rank = ranks[rand.Next(ranks.Length)];
                    string suit = suits[rand.Next(suits.Length)];
                    ConsoleColor color = (suit == "♥" || suit == "♦") ? ConsoleColor.Red : ConsoleColor.White;
                    Console.ForegroundColor = color;
                    line += $" {suit} {rank} ";
                }

                Console.Write(line);
                Thread.Sleep(120);
            }

            Console.ResetColor();*/
    }

    static int PromptInt(string label, int current)
    {
        WriteLineWithColor($"{label}: {current}  ➜  Enter new value or press Enter to keep", ConsoleColor.Gray);
        Console.Write(" > ");
        string input = Console.ReadLine()!;
        if (string.IsNullOrWhiteSpace(input)) return current;
        if (int.TryParse(input, out int result)) return result;

        WriteLineWithColor("❌ Invalid number, try again.", ConsoleColor.Red);
        return PromptInt(label, current);
    }

    static void WriteLineWithColor(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    static void P(string input)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(" " + input);
        Console.ResetColor();
    }

    static bool GetBool()
    {
        Console.Write(" (y/[n]): ");
        string input = Console.ReadLine()!;
        if (string.IsNullOrEmpty(input)) return false;

        input = input.ToLower();
        return input == "y" || input == "yes";
    }

    static int AskInt(string label, int current)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write($"{label} [{current}]: ");
        Console.ResetColor();
        string input = Console.ReadLine()!;
        return int.TryParse(input, out var val) ? val : current;
    }

    static (int start, int end) AskRange(string label, int current)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write($"{label} [{current}]: ");
        Console.ResetColor();
        string input = Console.ReadLine()!;

        if (string.IsNullOrWhiteSpace(input))
            return (current, current);

        if (input.Contains('-'))
        {
            var parts = input.Split('-');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int start) &&
                int.TryParse(parts[1], out int end))
            {
                return (start, end);
            }
        }
        else if (int.TryParse(input, out int val))
        {
            return (val, val);
        }

        WriteLineWithColor("❌ Invalid input. Use a number or range (e.g., 3-7)", ConsoleColor.Red);
        return AskRange(label, current);
    }


    static void PrintConfig()
    {
        P($"👥 Players: {PlayerRange.start}-{PlayerRange.end}");
        P($"🎴 Hole Cards: {HoleCardRange.start}-{HoleCardRange.end}");
        P($"🃏 Community Cards: {CommunityCardRange.start}-{CommunityCardRange.end}");
        P($"🔁 Rounds per Game: {RoundsPerGame}");
        P($"♻️ Total Games: {TotalGames}");
    }

    static bool cancelRequested = false;
    static bool shouldExit = false;

    static void SetupCtrlCHandler()
    {
        Console.CancelKeyPress += (sender, e) =>
        {
            if (cancelRequested)
            {
                // If already prompted once, second Ctrl+C exits immediately
                Console.WriteLine("\nForce cancelling...");
                Environment.Exit(0);
            }

            e.Cancel = true; // prevent immediate termination
            cancelRequested = true;

            Console.WriteLine("\n⚠ Really want to cancel? (y/n): ");
            var key = Console.ReadKey(intercept: true).Key;

            if (key == ConsoleKey.Y)
            {
                shouldExit = true;
            }
            else
            {
                cancelRequested = false;
                Console.WriteLine("\nResuming...");
            }
        };
    }

    public static void TestRoyalFlush()
    {
        // Create a hand with a royal flush in spades
        var royalFlush = new List<Card>
        {
            new Card(Suits.Spades, Ranks.King),
            new Card(Suits.Spades, Ranks.Jack),
            new Card(Suits.Spades, Ranks.Queen),
            new Card(Suits.Spades, Ranks.Ace),
            new Card(Suits.Spades, Ranks.Ten),
            new Card(Suits.Hearts, Ranks.Four), // filler
            new Card(Suits.Clubs, Ranks.Two) // filler
        };

        var hand = PokerHandEvaluator.EvaluateBestHand(royalFlush.ToArray());
        Console.WriteLine("Test Hand: " + string.Join(" ", royalFlush));
        Console.WriteLine("Detected Rank: " + hand.Rank);

        Debug.Assert(hand.Rank == HandRank.RoyalFlush, "❌ Failed to detect Royal Flush");
        Console.WriteLine("✅ Royal Flush detected correctly.");
    }

    public static bool HasWriteAccess(string directoryPath)
    {
        try
        {
            string testFile = Path.Combine(directoryPath, "permission_test.tmp");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
            return true;
        }
        catch
        {
            return false;
        }
    }
}