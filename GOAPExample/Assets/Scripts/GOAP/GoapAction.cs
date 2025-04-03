using System;
using System.Collections.Generic;

public sealed class GoapAction : IGoapAction
{
    private readonly HashSet<IBelief> mEffects;
    private readonly string mName;
    private readonly HashSet<IBelief> mPreconditions;

    private Func<int> mCostFunc;
    private IActionStrategy mStrategy;

    private GoapAction(string name)
    {
        mEffects = new HashSet<IBelief>();
        mName = name;
        mPreconditions = new HashSet<IBelief>();
    }

    public bool Completed => mStrategy?.Completed ?? true;

    public int Cost => mCostFunc?.Invoke() ?? 1;

    public IReadOnlyCollection<IBelief> Effects => mEffects;

    public string Name => mName;

    public IReadOnlyCollection<IBelief> PreConditions => mPreconditions;

    public void Start()
    {
        mStrategy?.Start();
    }

    public void Stop()
    {
        mStrategy?.Stop();
    }

    public void Update(float deltaTime)
    {
        if (mStrategy == null)
        {
            return;
        }

        if (mStrategy.CanPerform)
        {
            mStrategy.Update(deltaTime);
        }

        if (!Completed)
            return;

        foreach (var effect in Effects)
        {
            effect.Evaluate();
        }
    }

    public sealed class Builder
    {
        private readonly GoapAction mAction;

        public Builder(string name)
        {
            mAction = new GoapAction(name);
        }

        public Builder AddEffect(IBelief beliefs)
        {
            mAction.mEffects.Add(beliefs);
            return this;
        }

        public Builder AddPrecondition(IBelief beliefs)
        {
            mAction.mPreconditions.Add(beliefs);
            return this;
        }

        public Builder WithCost(int cost)
        {
            mAction.mCostFunc = () => cost;
            return this;
        }

        public Builder WithCost(Func<int> costFunc)
        {
            mAction.mCostFunc = costFunc;
            return this;
        }

        public Builder WithStrategy(IActionStrategy strategy)
        {
            mAction.mStrategy = strategy;
            return this;
        }

        public GoapAction Build()
        {
            return mAction;
        }
    }
}
