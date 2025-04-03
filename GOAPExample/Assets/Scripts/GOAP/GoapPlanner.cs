using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IGoapPlanner {
    ActionPlan Plan(ICollection<IGoapAction> actions, HashSet<IGoapGoal> goals, IGoapGoal mostRecentGoal = null);
}

public class GoapPlanner : IGoapPlanner {
    public ActionPlan Plan(ICollection<IGoapAction> actions, HashSet<IGoapGoal> goals, IGoapGoal mostRecentGoal = null) {
        List<IGoapGoal> orderedGoals = goals
            .Where(g => g.DesiredEffects.Any(b => !b.Evaluate()))
            .OrderByDescending(g => g == mostRecentGoal ? g.Priority - 0.01 : g.Priority)
            .ToList();

        foreach (var goal in orderedGoals) {
            var goalNode = new Node(null, null, new HashSet<IBelief>(goal.DesiredEffects), 0);
            if (FindPathAStar(goalNode, actions, out var path)) {
                return new ActionPlan(goal, new Stack<IGoapAction>(path), path.Sum(a => a.Cost));
            }
        }

        Debug.LogWarning("No plan found");
        return null;
    }

    private bool FindPathAStar(Node startNode, ICollection<IGoapAction> actions, out List<IGoapAction> actionPath) {
        var openSet = new SortedSet<Node>(new NodeComparer());
        var closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node current = openSet.First();
            openSet.Remove(current);
            closedSet.Add(current);
            
            if (current.RequiredEffects.Count == 0) {
                actionPath = ReconstructPath(current);
                return true;
            }

            foreach (var action in actions) {
                if (!action.Effects.Any(current.RequiredEffects.Contains)) continue;

                var newRequiredEffects = new HashSet<IBelief>(current.RequiredEffects);
                newRequiredEffects.ExceptWith(action.Effects);
                newRequiredEffects.UnionWith(action.PreConditions);

                var newNode = new Node(current, action, newRequiredEffects, current.Cost + action.Cost);

                if (closedSet.Contains(newNode)) continue;
                if (!openSet.Contains(newNode)) openSet.Add(newNode);
            }
        }

        actionPath = null;
        return false;
    }

    private List<IGoapAction> ReconstructPath(Node node) {
        var path = new List<IGoapAction>();
        while (node.Action != null) {
            path.Add(node.Action);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }
}

public class Node {
    public Node Parent { get; }
    public IGoapAction Action { get; }
    public HashSet<IBelief> RequiredEffects { get; }
    public float Cost { get; }
    
    public Node(Node parent, IGoapAction action, HashSet<IBelief> effects, float cost) {
        Parent = parent;
        Action = action;
        RequiredEffects = effects;
        RequiredEffects.RemoveWhere(e => e.Evaluate());
        Cost = cost;
    }
}

public class NodeComparer : IComparer<Node> {
    public int Compare(Node x, Node y) {
        return x.Cost.CompareTo(y.Cost);
    }
}

public class ActionPlan {
    public IGoapGoal AgentGoal { get; }
    public Stack<IGoapAction> Actions { get; }
    public float TotalCost { get; set; }
    
    public ActionPlan(IGoapGoal goal, Stack<IGoapAction> actions, float totalCost) {
        AgentGoal = goal;
        Actions = actions;
        TotalCost = totalCost;
    }
}