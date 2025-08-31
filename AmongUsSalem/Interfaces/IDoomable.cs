namespace AmongUsSalem.Extensions;

public interface IDoomable
{
    DoomableType DoomHintType { get; }
}

public enum DoomableType
{
    Default,
    Perception,
    Insight,
    Death,
    Hunter,
    Fearmonger,
    Protective,
    Trickster,
    Relentless
}