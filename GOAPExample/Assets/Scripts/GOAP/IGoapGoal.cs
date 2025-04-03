using System.Collections.Generic;

public interface IGoapGoal
{
    IReadOnlyCollection<IBelief> DesiredEffects { get; }
    string Name { get; }
    int Priority { get; }
}