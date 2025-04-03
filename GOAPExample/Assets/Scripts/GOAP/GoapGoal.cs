using System;
using System.Collections.Generic;

public sealed class GoapGoal : IGoapGoal
{
    private readonly HashSet<IBelief> mDesiredEffects;
    private readonly string mName;
    
    private Func<int> mPriorityFunc;

    private GoapGoal(string name)
    {
        mDesiredEffects = new HashSet<IBelief>();
        mName = name;
    }

    public IReadOnlyCollection<IBelief> DesiredEffects => mDesiredEffects;

    public string Name => mName;

    public int Priority => mPriorityFunc?.Invoke() ?? 1;

    public sealed class Builder
    {
        private readonly GoapGoal mGoal;

        public Builder(string name)
        {
            mGoal = new GoapGoal(name);
        }

        public Builder WithDesiredEffect(IBelief beliefs)
        {
            mGoal.mDesiredEffects.Add(beliefs);
            return this;
        }

        public Builder WithPriority(int priority)
        {
            mGoal.mPriorityFunc = () => priority;
            return this;
        }

        public Builder WithPriority(Func<int> priorityFunc)
        {
            mGoal.mPriorityFunc = priorityFunc;
            return this;
        }

        public GoapGoal Build()
        {
            return mGoal;
        }
    }
}