using PokerSim.Enums;
using PokerSim.models;

namespace PokerSim.Dto;

public class HandDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class SimulationStatsDto
{
    public int Players { get; set; }
    public int HoleCards { get; set; }
    public int CommunityCards { get; set; }
    public int RoundsPlayed { get; set; }
    public List<HandDto> TotalHands { get; set; } = [];
    public List<HandDto> WinningHands { get; set; } = [];
}

public static class HandMapper
{
    public static Hand Map(HandDto dto)
    {
        if (!Enum.TryParse<HandRank>(dto.Name, out var rank))
        {
            throw new InvalidOperationException($"Unknown hand name: {dto.Name}");
        }

        return new Hand(rank, dto.Count);
    }
}