using PokerSim.models;

namespace PokerSim.Logic;

using PokerSim.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

public static class PokerHandEvaluator
{
    public static HandRank EvaluateBestHand(Card[] cards)
    {
        if (cards.Length < 5)
            throw new ArgumentException("At least 5 cards are required to evaluate a poker hand.");

        HandRank bestRank = HandRank.HighCard;

        foreach (var combo in GetCombinations(cards, 5))
        {
            var rank = EvaluateFiveCardHand(combo);
            if (rank > bestRank)
                bestRank = rank;
        }

        return bestRank;
    }

    private static HandRank EvaluateFiveCardHand(Card[] hand)
    {
        var ranks = hand.Select(c => (int)c.Rank).ToList();
        var suits = hand.Select(c => c.Suit).ToList();

        ranks.Sort();

        bool isFlush = suits.Distinct().Count() == 1;
        bool isStraight = IsStraight(ranks);

        var groups = ranks.GroupBy(r => r).Select(g => g.Count()).OrderByDescending(c => c).ToList();

        if (isFlush && isStraight && ranks.Max() == (int)Ranks.Ace)
            return HandRank.RoyalFlush;
        if (isFlush && isStraight)
            return HandRank.StraightFlush;
        if (groups[0] == 4)
            return HandRank.FourOfAKind;
        if (groups[0] == 3 && groups[1] == 2)
            return HandRank.FullHouse;
        if (isFlush)
            return HandRank.Flush;
        if (isStraight)
            return HandRank.Straight;
        if (groups[0] == 3)
            return HandRank.ThreeOfAKind;
        if (groups[0] == 2 && groups[1] == 2)
            return HandRank.TwoPair;
        if (groups[0] == 2)
            return HandRank.OnePair;

        return HandRank.HighCard;
    }

    private static bool IsStraight(List<int> sortedRanks)
    {
        var distinct = sortedRanks.Distinct().ToList();
        if (distinct.Count < 5)
            return false;

        for (int i = 0; i <= distinct.Count - 5; i++)
        {
            if (distinct[i + 4] - distinct[i] == 4)
                return true;
        }

        // Special case: A-2-3-4-5 (Ace as low)
        return distinct.Contains((int)Ranks.Ace) &&
               distinct.Contains((int)Ranks.Two) &&
               distinct.Contains((int)Ranks.Three) &&
               distinct.Contains((int)Ranks.Four) &&
               distinct.Contains((int)Ranks.Five);
    }

    private static IEnumerable<Card[]> GetCombinations(Card[] cards, int choose)
    {
        int n = cards.Length;
        int[] indices = Enumerable.Range(0, choose).ToArray();

        while (true)
        {
            // Yield current combination
            yield return indices.Select(i => cards[i]).ToArray();

            // Find the rightmost index that can be incremented
            int i = choose - 1;
            while (i >= 0 && indices[i] == i + n - choose)
                i--;

            if (i < 0)
                yield break;

            indices[i]++;
            for (int j = i + 1; j < choose; j++)
                indices[j] = indices[j - 1] + 1;
        }
    }
}