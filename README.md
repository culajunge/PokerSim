# ♠️♥️PokerSim♣️♦️ — Poker Hand Simulation and Statistics

PokerSim is a console application to simulate poker hands for various game variants and player counts, gathering
detailed statistics about hand occurrences, winning probabilities, and performance metrics. It helps poker players,
enthusiasts, and developers understand the odds and win rates of different hands under custom game rules.

---

## Features

- Simulate Texas Hold’em variants with customizable players, hole cards, and community cards.
- Collect and analyze statistics for total hand occurrences, winning hands, and win efficiencies.
- Generate insightful charts and summaries to guide decision-making.
- Supports exporting and reading simulation results as JSON files for offline review.

---

## Installation

1. Go to the [Releases page](https://github.com/yourusername/PokerSim/releases) of this repository.
2. Download the latest release ZIP or TAR.GZ archive.
3. Extract the archive to your preferred folder.
4. Inside, you'll find the executable file `PokerSim` (Linux/Mac).
5. **Note:** The application requires the [.NET 7 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) or
   newer to be installed on your system.
    - On Linux/Mac, follow the official .NET install instructions linked above.

---

## Running the Application

Open a terminal or command prompt, navigate to the extracted folder, then run:

```bash
./PokerSim    # Linux/Mac
```

Running a Simulation
--------------------

1. Select the option to start a new simulation.

2. Enter the number of players (e.g., 6).

3. Specify hole cards per player (e.g., 2).

4. Specify community cards (e.g., 5).

5. Enter the number of rounds (hands) to simulate (e.g., 40,000).

6. The simulation will run, displaying progress, then save the results to a JSON file.

Accessing and Reviewing Simulation Results
------------------------------------------

* Use the “Load Simulation” option to open a saved JSON results file.

```bash
♥ Poker Simulation Results

Players: 5
HoleCards: 2
CommunityCards: 5
Rounds played: 40000
Hands played: 200000
Cards played: 600000
Cheers

🃏 Hand Occurrences:
Amount of specific Hand out of all Hands
HighCard      |█████░░░░░░░░░░░░░░░░░░░░░░░░░| 17.142% (34285)
OnePair       |████████████░░░░░░░░░░░░░░░░░░| 43.33% (86661)
TwoPair       |███████░░░░░░░░░░░░░░░░░░░░░░░| 23.634% (47269)
ThreeOfAKind  |█░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 5.026% (10052)
Straight      |█░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 4.554% (9108)
Flush         |░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 3.178% (6355)
FullHouse     |░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 2.857% (5713)
FourOfAKind   |░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 0.245% (490)
StraightFlush |░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 0.03% (60)
RoyalFlush    |░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 0.004% (7)

🏆 Hand Win Rates:
Amount of Wins gathered by specific Hand
HighCard      |░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 0.158% (63) -              
OnePair       |█████░░░░░░░░░░░░░░░░░░░░░░░░░| 19.058% (7623) -              
TwoPair       |█████████░░░░░░░░░░░░░░░░░░░░░| 31.435% (12574) -              
ThreeOfAKind  |███░░░░░░░░░░░░░░░░░░░░░░░░░░░| 12.855% (5142) -              
Straight      |████░░░░░░░░░░░░░░░░░░░░░░░░░░| 14.9% (5960) -              
Flush         |███░░░░░░░░░░░░░░░░░░░░░░░░░░░| 10% (4000) -              
FullHouse     |███░░░░░░░░░░░░░░░░░░░░░░░░░░░| 10.442% (4177) -              
FourOfAKind   |░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 0.985% (394) -              
StraightFlush |░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 0.15% (60) -              
RoyalFlush    |░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 0.018% (7) -

⚖  Win Efficiency:
Amount of Wins gathered by specific Hand out of all those specific Hand
HighCard      |░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 0.184% (63/34285)  
OnePair       |██░░░░░░░░░░░░░░░░░░░░░░░░░░░░| 8.796% (7623/86661)  
TwoPair       |███████░░░░░░░░░░░░░░░░░░░░░░░| 26.601% (12574/47269)  
ThreeOfAKind  |███████████████░░░░░░░░░░░░░░░| 51.154% (5142/10052)  
Straight      |███████████████████░░░░░░░░░░░| 65.437% (5960/9108)  
Flush         |██████████████████░░░░░░░░░░░░| 62.943% (4000/6355)  
FullHouse     |█████████████████████░░░░░░░░░| 73.114% (4177/5713)  
FourOfAKind   |████████████████████████░░░░░░| 80.408% (394/490)  
StraightFlush |██████████████████████████████| 100% (60/60)  
RoyalFlush    |██████████████████████████████| 100% (7/7)

🎯 Win Share (% of total wins by hand):
----------------------------------------
1. TwoPair            31.44 %
2. OnePair            19.06 %
3. Straight           14.90 %
4. ThreeOfAKind       12.86 %
5. FullHouse          10.44 %
6. Flush              10.00 %
7. FourOfAKind         0.98 %
8. HighCard            0.16 %
9. StraightFlush       0.15 %
10. RoyalFlush         0.02 %
Values greater than one considered lucky :), values less than one considered unlucky :(

📊 Overperformance:
--------------------------------------------------
Hand            Occ%   Win% Luck   Chart
--------------------------------------------------
OnePair         43%   19%    0.4   
TwoPair         24%   31%    1.3   --
HighCard        17%    0%    0.0   
ThreeOfAKind     5%   13%    2.6   +++++
Straight         5%   15%    3.3   ++++++
Flush            3%   10%    3.1   ++++++
FullHouse        3%   10%    3.7   +++++++
FourOfAKind      0%    1%    4.0   ++++++++
StraightFlush    0%    0%    5.0   ++++++++++
RoyalFlush       0%    0%    5.0   +++++++++


📊 Summary:
----------------------------------------
Average hand: OnePair (86661 times)
Average winning hand: TwoPair (12574 wins)
Safe advantage hand (≥50% win ratio): ThreeOfAKind (51.15%)

```

How to Use the Results
----------------------

* **Improve Decision-Making:** By knowing which hands are more likely to win or overperform, you can adjust your playing
  strategy—deciding when to fold, call, or raise.

* **Understand Variants:** Results adapt to your custom game variant parameters, making it easier to analyze less common
  poker variants.

* **Detect Anomalies:** Spot unexpected patterns or simulation biases by comparing multiple runs.

* **Develop Bots/AI:** Use the statistics to train or fine-tune poker-playing AI or strategy analysis tools.

Notes
-----

* Simulation accuracy improves with higher rounds but requires more time.

* Rare hands may show volatile statistics due to fewer occurrences.

* Results reflect the simulated variant rules and number of players and should be interpreted accordingly.

Contributing
------------

Contributions and improvements are welcome! Please open issues or submit pull requests.

License
-------

MIT License — see LICENSE for details.