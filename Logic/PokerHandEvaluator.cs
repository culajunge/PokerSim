using PokerSim.models;

namespace PokerSim.Logic;

using PokerSim.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

public static class PokerHandEvaluator
{
    public static EvaluatedHand EvaluateBestHand(Card[] cards)
    {
        /*
        if (cards.Length < 5)
            throw new ArgumentException("At least 5 cards are required to evaluate a poker hand.");*/

        EvaluatedHand bestHand = null;

        foreach (var combo in GetCombinations(cards, 5))
        {
            var rank = EvaluateFiveCardHand(combo);
            var ordered = combo.OrderByDescending<Card, object>(c => c.Rank).ToArray();
            var kickers = cards.Except(ordered).OrderByDescending<Card, object>(c => c.Rank).ToArray();

            var current = new EvaluatedHand(rank.Rank, ordered, kickers);

            if (bestHand == null || current.CompareTo(bestHand) > 0)
            {
                bestHand = current;
            }
        }

        return bestHand;
    }

    private static EvaluatedHand CompareTieBreaker(Card[] a, Card[] b)
    {
        var evalA = EvaluateBestHand(a);
        var evalB = EvaluateBestHand(b);

        int result = evalA.CompareTo(evalB);
        if (result > 0) return evalA;
        else return evalB;
    }


    private static HandEvaluation EvaluateFiveCardHand(Card[] hand)
    {
        var ranks = hand.Select(c => (int)c.Rank).ToList();
        var suits = hand.Select(c => c.Suit).ToList();

        var grouped = ranks
            .GroupBy(r => r)
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Key)
            .ToList();

        var rankCounts = grouped.Select(g => g.Count()).ToList();
        var sortedRanks = grouped.Select(g => g.Key).ToList();

        bool isFlush = suits.Distinct().Count() == 1;
        bool isStraight = IsStraight(ranks, out int highCard);

        if (isFlush && isStraight && highCard == 14)
            return new HandEvaluation { Rank = HandRank.RoyalFlush, TieBreakers = new List<int> { 14 } };

        if (isFlush && isStraight)
            return new HandEvaluation { Rank = HandRank.StraightFlush, TieBreakers = new List<int> { highCard } };

        if (rankCounts[0] == 4)
            return new HandEvaluation
                { Rank = HandRank.FourOfAKind, TieBreakers = new List<int> { sortedRanks[0], sortedRanks[1] } };

        if (rankCounts[0] == 3 && rankCounts[1] == 2)
            return new HandEvaluation
                { Rank = HandRank.FullHouse, TieBreakers = new List<int> { sortedRanks[0], sortedRanks[1] } };

        if (isFlush)
            return new HandEvaluation { Rank = HandRank.Flush, TieBreakers = ranks.OrderByDescending(r => r).ToList() };

        if (isStraight)
            return new HandEvaluation { Rank = HandRank.Straight, TieBreakers = new List<int> { highCard } };

        if (rankCounts[0] == 3)
            return new HandEvaluation
            {
                Rank = HandRank.ThreeOfAKind,
                TieBreakers = new List<int> { sortedRanks[0], sortedRanks[1], sortedRanks[2] }
            };

        if (rankCounts[0] == 2 && rankCounts[1] == 2)
            return new HandEvaluation
            {
                Rank = HandRank.TwoPair, TieBreakers = new List<int> { sortedRanks[0], sortedRanks[1], sortedRanks[2] }
            };

        if (rankCounts[0] == 2)
            return new HandEvaluation
            {
                Rank = HandRank.OnePair,
                TieBreakers = new List<int> { sortedRanks[0] }.Concat(sortedRanks.Skip(1)).ToList()
            };

        return new HandEvaluation { Rank = HandRank.HighCard, TieBreakers = ranks.OrderByDescending(r => r).ToList() };
    }

    private static bool IsStraight(List<int> ranks, out int highCard)
    {
        var distinct = ranks.Distinct().OrderBy(r => r).ToList();
        highCard = 0;

        for (int i = 0; i <= distinct.Count - 5; i++)
        {
            if (distinct[i + 4] - distinct[i] == 4)
            {
                highCard = distinct[i + 4];
                return true;
            }
        }

        // Special case: A-2-3-4-5
        if (distinct.Contains(14) && distinct.Take(4).SequenceEqual(new[] { 2, 3, 4, 5 }))
        {
            highCard = 5;
            return true;
        }

        return false;
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