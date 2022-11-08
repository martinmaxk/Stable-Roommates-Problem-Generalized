using System.Collections;
using Optimization_Roommates_Problem_CS.NeighborGenerators;
using Optimization_Roommates_Problem_CS.Objectives;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS.HillClimbing;

public abstract class BaseHillClimber
{
    protected int[][] groups, weights;
    protected IObjective objective;
    protected INeighborGenerator neighborGenerator;
    protected BitArray isInGroupIndices1, isInGroupIndices2;
    
    public double Value => objective.Value;
    public abstract int NumSteps { get; }
    public abstract int NumNeighbors { get; }

    public void Initialize(int[][] groups, int[][] weights, IObjective objective)
    {
        this.groups = groups;
        this.weights = weights;
        this.objective = objective;
        objective.Initialize(groups, weights);
        ArrayUtils.CreateOrResize(ref isInGroupIndices1, groups.Length, true);
        ArrayUtils.CreateOrResize(ref isInGroupIndices2, groups.Length, true);
        OnInitialize();
    }
    
    protected virtual void OnInitialize()
    { }

    /// <summary>
    /// Tries to select a neighbor as next state
    /// </summary>
    /// <returns>Are there any more steps to process?</returns>
    public abstract bool NextStep();

    public int[][] GetGroups()
    {
        return groups;
    }
    
    protected void ApplyDelta(NeighborDelta neighborDelta)
    {
        objective.ApplyDelta(groups, weights, neighborDelta);
        neighborDelta.Apply(groups);
    }
    
    protected void UnapplyDelta(NeighborDelta neighborDelta)
    {
        objective.UnapplyDelta(groups, weights, neighborDelta);
        neighborDelta.Unapply(groups);  
    }
}


