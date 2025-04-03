using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public sealed class Agent : MonoBehaviour, IAgent
{
    private readonly HashSet<IGoapAction> mActions = new HashSet<IGoapAction>();
    private readonly IDictionary<string, IBelief> mBeliefs = new Dictionary<string, IBelief>();
    private readonly IList<Vector2Int> mCollectedGemLocations = new List<Vector2Int>();
    private readonly HashSet<IGoapGoal> mGoals = new HashSet<IGoapGoal>();
    private readonly IGoapPlanner mGoapPlanner = new GoapPlanner();

    [SerializeField] private NavMeshAgent _navMeshAgent;

    private AgentStats mAgentStats;
    private IGoapAction mCurrentAction;
    private IGemDiscoverySystem mGemDiscoverySystem;
    private IGemManager mGemManager;
    private IGridDiscoverySystem mGridDiscoverySystem;
    private IGridManager mGridManager;
    private ActionPlan mPlan;
    private IGoapGoal mRecentGoal;

    public Vector3 Position => transform.position;

    public void Initialize(
        AgentStats agentStats,
        IGemDiscoverySystem gemDiscoverySystem,
        IGemManager gemManager,
        IGridDiscoverySystem gridDiscoverySystem,
        IGridManager gridManager)
    {
        mAgentStats = agentStats;
        mGemDiscoverySystem = gemDiscoverySystem;
        mGemManager = gemManager;
        mGridDiscoverySystem = gridDiscoverySystem;
        mGridManager = gridManager;

        mAgentStats.UpdateGemCount(mCollectedGemLocations.Count);
    }

    public void OnGemCollected(Vector2Int index)
    {
        mCollectedGemLocations.Add(index);
        mAgentStats.UpdateGemCount(mCollectedGemLocations.Count);
        _navMeshAgent.avoidancePriority = mCollectedGemLocations.Count + 50;
        _navMeshAgent.speed = 1f / (mCollectedGemLocations.Count * 0.5f + 1f);
    }

    private void SetupActions()
    {
        mActions.Add(new GoapAction.Builder("Move")
        .AddEffect(mBeliefs["AgentMoving"])
        .WithStrategy(new MoveToLocationStrategy(mGridManager, GetRandomEmptyCell, _navMeshAgent))
        .Build());

        mActions.Add(new GoapAction.Builder("MoveNearToGem")
        .AddEffect(mBeliefs["CanCollectGem"])
        .AddPrecondition(mBeliefs["GemsAvailableToCollect"])
        .WithStrategy(new MoveToLocationStrategy(mGridManager, GetGemPosition, _navMeshAgent))
        .Build());

        mActions.Add(new GoapAction.Builder("MoveNearToBreakableBlock")
        .AddEffect(mBeliefs["CanBreakBlock"])
        .AddPrecondition(mBeliefs["BlocksAvailableToBreak"])
        .WithStrategy(new MoveToLocationStrategy(mGridManager, GetBreakablePosition, _navMeshAgent))
        .Build());

        mActions.Add(new GoapAction.Builder("CollectGem")
        .AddEffect(mBeliefs["GemCollected"])
        .AddPrecondition(mBeliefs["GemsAvailableToCollect"])
        .AddPrecondition(mBeliefs["CanCollectGem"])
        .WithStrategy(new CollectGemStrategy(this, mGemDiscoverySystem, GetGemIndexToCollect, mGemManager))
        .Build());

        mActions.Add(new GoapAction.Builder("BreakBlock")
        .AddEffect(mBeliefs["BlockBroke"])
        .AddPrecondition(mBeliefs["BlocksAvailableToBreak"])
        .AddPrecondition(mBeliefs["CanBreakBlock"])
        .WithStrategy(new BreakBlockStrategy(this, mGridDiscoverySystem, mGridManager))
        .Build());
    }

    private void SetupBeliefs()
    {
        mBeliefs.Add("Nothing", new Belief("Nothing", () => false));
        mBeliefs.Add("AgentIdle", new Belief("AgentIdle", () => !_navMeshAgent.hasPath));
        mBeliefs.Add("AgentMoving", new Belief("AgentMoving", () => _navMeshAgent.hasPath));
        mBeliefs.Add("GemsAvailableToCollect", new Belief("GemsAvailableToCollect", () => mGemDiscoverySystem.DiscoveredGemLocations.Count > 0 && mGemDiscoverySystem.DiscoveredGemLocations.Any(index => IsLocationReachable(index, _navMeshAgent.radius, out _))));
        mBeliefs.Add("BlocksAvailableToBreak", new Belief("BlocksAvailableToBreak", () => mGridDiscoverySystem.DiscoveredBreakableBlockLocations.Count > 0 && mGridDiscoverySystem.DiscoveredBreakableBlockLocations.Any(index => IsLocationReachable(index, 1.1f, out _))));
        mBeliefs.Add("CanCollectGem", new Belief("CanCollectGem", () => mGemManager.TryGetNearbyGemIndex(Position.ToIndex2D(mGridManager.GridSize), 0.3f, out _)));
        mBeliefs.Add("CanBreakBlock", new Belief("CanBreakBlock", () => mGridManager.TryGetNearbyBreakableIndex(Position.ToIndex2D(mGridManager.GridSize), 1.1f, out _)));
        mBeliefs.Add("GemCollected", new Belief("GemCollected", () => mGemDiscoverySystem.DiscoveredGemLocations.Count == 0));
        mBeliefs.Add("BlockBroke", new Belief("BlockBroke", () => mGridDiscoverySystem.DiscoveredBreakableBlockLocations.Count == 0));
    }

    private void SetupGoals()
    {
        mGoals.Add(new GoapGoal.Builder("Explore")
        .WithDesiredEffect(mBeliefs["AgentMoving"])
        .Build());

        mGoals.Add(new GoapGoal.Builder("SearchForGems")
        .WithDesiredEffect(mBeliefs["BlockBroke"])
        .WithPriority(2)
        .Build());

        mGoals.Add(new GoapGoal.Builder("CollectGems")
        .WithDesiredEffect(mBeliefs["GemCollected"])
        .WithPriority(3)
        .Build());
    }

    private void Start()
    {
        _navMeshAgent.avoidancePriority = Random.Range(0, 50);
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    private void Update()
    {
        if (mPlan == null)
            GetNextPlan();

        if (mPlan == null)
        {
            return;
        }

        if (mCurrentAction == null)
        {
            if (mPlan.Actions.Count > 0)
            {
                mCurrentAction = mPlan.Actions.Pop();
                if (mCurrentAction.PreConditions.All(p => p.Evaluate()))
                {
                    mCurrentAction.Start();
                }
                else
                {
                    mPlan = null;
                    mCurrentAction = null;
                }
            }
            else
            {
                mPlan = null;
            }
        }

        if (mCurrentAction != null)
        {
            mCurrentAction.Update(Time.deltaTime);
            if (!mCurrentAction.Completed)
                return;

            foreach (var effect in mCurrentAction.Effects)
            {
                effect.Evaluate();
            }

            mCurrentAction.Stop();
            mCurrentAction = null;
        }
    }

    private void GetNextPlan()
    {
        mPlan = mGoapPlanner.Plan(mActions, mGoals, mRecentGoal);
        if (mPlan != null)
        {
            mRecentGoal = mPlan.AgentGoal;
            Debug.Log(mPlan.AgentGoal.Name);
        }
    }

    private Vector2Int GetGemPosition()
    {
        var gemIndex = mGemDiscoverySystem.DiscoveredGemLocations.ElementAt(Random.Range(0, mGemDiscoverySystem.DiscoveredGemLocations.Count));
        return gemIndex;
    }

    private Vector2Int GetGemIndexToCollect()
    {
        return mGemDiscoverySystem.DiscoveredGemLocations.Where(index => Vector2Int.Distance(index, Position.ToIndex2D(mGridManager.GridSize)) <= 0.1f).FirstOrDefault();
    }

    private Vector2Int GetBreakablePosition()
    {
        var breakables = mGridDiscoverySystem.DiscoveredBreakableBlockLocations.OrderBy(index => Vector2Int.Distance(index, Position.ToIndex2D(mGridManager.GridSize)));
        foreach (var index in breakables)
        {
            if (IsLocationReachable(index, 1.1f, out var result))
            {
                return result;
            }
        }

        return default;
    }

    private Vector2Int GetRandomEmptyCell()
    {
        return mGridManager.AvailableCells.ElementAt(Random.Range(0, mGridManager.AvailableCells.Count));
    }

    private bool IsLocationReachable(Vector2Int index, float range, out Vector2Int result)
    {
        if (mGridManager.TryGetNearbyCellIndexes(index, range, out var indexes))
        {
            foreach (var i in indexes)
            {
                NavMeshPath path = new();
                if (NavMesh.CalculatePath(Position, i.ToPosition(mGridManager.GridSize), NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    result = i;
                    return true;
                }
            }
        }

        result = default;
        return false;
    }
}
