using System;

public sealed class Belief : IBelief
{
    private readonly Func<bool> mConditionFunc = () => false;
    private readonly string mName;

    public Belief(string name, Func<bool> conditionFunc)
    {
        mConditionFunc = conditionFunc;
        mName = name;
    }

    public string Name => mName;

    public bool Evaluate()
    {
        return mConditionFunc();
    }
}
