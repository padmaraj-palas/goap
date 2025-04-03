public interface IActionStrategy
{
    bool CanPerform { get; }
    bool Completed { get; }

    void Start();
    void Stop();
    void Update(float deltaTime);
}
