using System.Collections.Generic;

public interface IGoapAction
{
    bool Completed { get; }
    int Cost { get; }
    IReadOnlyCollection<IBelief> Effects { get; }
    string Name { get; }
    IReadOnlyCollection<IBelief> PreConditions { get; }

    void Start();
    void Stop();
    void Update(float deltaTime);
}
