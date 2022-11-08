using Google.OrTools.LinearSolver;
using System.Collections.Generic;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS;

public class RoommatesLpSolver
{
    private MPSolverParameters solverParams;
    
    private int n, k;
    
    public RoommatesLpSolver()
    {
        solverParams = new MPSolverParameters();
        solverParams.SetIntegerParam(MPSolverParameters.IntegerParam.LP_ALGORITHM, 
            (int)MPSolverParameters.LpAlgorithmValues.BARRIER);    
    }
    
    public Solver InitializeUtil(int k, int[][] weights)
    {
        // See https://developers.google.com/optimization/lp/lp_advanced
        // for alternative solver names
        var solver = Solver.CreateSolver("Clp");

        n = weights.Length;
        this.k = k;
        AddGroupVarsAndConstraints(solver, n, k);
        var variables = solver.variables();

        //Console.WriteLine("Number of constraints = " + solver.NumConstraints());

        var objectiveExpr = new LinearExpr();
        int zStartIdx = n * k;
        int zIndex = zStartIdx;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i != j)
                {
                    var z_ij = variables[zIndex];
                    // Assumes n is the largest weight
                    int invWeight = n - weights[i][j];
                    objectiveExpr += (z_ij * invWeight) + z_ij;
                }
                zIndex++;
            }
        }
        solver.Maximize(objectiveExpr);
        //Console.WriteLine(solver.ExportModelAsLpFormat(false));
        return solver;
    }
    
    public Solver InitializeDeon(int k, int[][] weights)
    {
        // See https://developers.google.com/optimization/lp/lp_advanced
        // for alternative solver names
        var solver = Solver.CreateSolver("Clp");

        n = weights.Length;
        this.k = k;
        AddGroupVarsAndConstraints(solver, n, k);
        var S = solver.MakeNumVar(double.NegativeInfinity, double.PositiveInfinity, "S");
        var variables = solver.variables();
        
        int zStartIdx = n * k;
        int zIndex = zStartIdx;
        for (int i = 0; i < n; i++)
        {
            var sumExpr = new LinearExpr();
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                    goto next;
                var z_ij = variables[zIndex];
                int invWeight = n - weights[i][j];
                sumExpr += z_ij * invWeight;
                next:
                zIndex++;
            }
            solver.Add(S <= sumExpr);
        }

        //Console.WriteLine("Number of constraints = " + solver.NumConstraints());

        LinearExpr objectiveExpr = S;
        zIndex = zStartIdx;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i != j)
                    objectiveExpr += variables[zIndex];
                zIndex++;
            }
        }
        solver.Maximize(objectiveExpr);
        //Console.WriteLine(solver.ExportModelAsLpFormat(false));
        return solver;
    }

    private void AddGroupVarsAndConstraints(Solver solver, int n, int k)
    {
        int groupSize = n / k;
        // x_im = 1 if person i is in group m, o.w. 0
        for (int i = 0; i < n; i++)
            for (int m = 0; m < k; m++)
                solver.MakeNumVar(0.0, 1.0, "x(" + i + ", " + m + ")");
        int zStartIdx = n * k;
        // z_ij = 1 if person i is in group with person j, o.w. 0
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                solver.MakeNumVar(double.NegativeInfinity, double.PositiveInfinity, 
                    "z(" + i + ", " + j + ")");
        //Console.WriteLine("Number of variables = " + solver.NumVariables());

        var variables = solver.variables();
        // Person i is in exactly one group
        for (int i = 0; i < n; i++)
        {
            var sumExpr = new LinearExpr();
            for (int m = 0; m < k; m++)
            {
                var x_im = variables[m + (i * k)];
                sumExpr += x_im;
            }
            solver.Add(sumExpr == 1);
        }
        
        // Every group has the same size
        for (int m = 0; m < k; m++)
        {
            var sumExpr = new LinearExpr();
            for (int i = 0; i < n; i++)
            {
                var x_im = variables[m + (i * k)];
                sumExpr += x_im;
            }
            solver.Add(sumExpr == groupSize);
        }
        
        int zIndex = zStartIdx;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                    goto next;
                var z_ij = variables[zIndex];
                for (int m = 0; m < k; m++)
                {
                    var x_im = variables[m + (i * k)];
                    var x_jm = variables[m + (j * k)];
                    solver.Add(z_ij <= 1 + x_im - x_jm);
                    solver.Add(z_ij <= 1 + x_jm - x_im);
                }
                next:
                zIndex++;
            }
        }
    }

    public double? Solve(Solver solver)
    {
        solver.Reset();
        var resultStatus = solver.Solve(solverParams);
        if (resultStatus != Solver.ResultStatus.OPTIMAL)
        {
            //Console.WriteLine(solver.ExportModelAsLpFormat(false));
            //Console.WriteLine("The problem does not have an optimal solution!");
            return null;
        }
        /*Console.WriteLine("Solution:");
        Console.WriteLine("Objective value = " + solver.Objective().Value());

        Console.WriteLine("Problem solved in " + solver.WallTime() + " milliseconds");
        Console.WriteLine("Problem solved in " + solver.Iterations() + " iterations");*/
        return solver.Objective().Value();
    }

    public void ReadResult(Solver solver, double[] result)
    {
        var variables = solver.variables();
        for (int i = 0; i < variables.Count; i++)
            result[i] = variables[i].SolutionValue();
    }

    public void GetX<T>(T[] variableResults, T[][] result)
    {
        // Read x variables over into result
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < k; j++)
                result[i][j] = variableResults[j + (i * k)];
        }
    }
    
    public void GetZ<T>(T[] variableResults, T[][] result)
    {
        int zIndex = n * k;
        // Read z variables over into result
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                result[i][j] = variableResults[zIndex++];
        }
    }
    
    private void AddConstraints(Solver solver, List<LinearConstraint> constraints)
    {
        for (int i = 0; i < constraints.Count; i++)
            solver.Add(constraints[i]);
    }
    
    private void PopConstraints(Solver solver, int popCount)
    {
        var solverConstraints = solver.constraints();
        solverConstraints.RemoveRange(solverConstraints.Count - popCount, popCount);
    }

    private int TryFindNonIntVarIndex(IList<Variable> variables)
    {
        //find non-integral value
        for (int i = 0; i < variables.Count; i++)
        {
            if (!NumericUtils.IsIntegral(variables[i].SolutionValue(), Epsilon))
                return i;
        }
        return -1;
    }

    private BranchAndBoundNode? CreateNode(Solver solver,
        Variable[] branchVars, List<LinearConstraint> extraConstraints)
    {
        double? objValueRes = Solve(solver);
        if (!objValueRes.HasValue)
            return null;
        int branchVarIndex = TryFindNonIntVarIndex(branchVars);
        return new BranchAndBoundNode(extraConstraints, objValueRes.Value, branchVarIndex,
            branchVarIndex == -1 ? double.NaN : branchVars[branchVarIndex].SolutionValue());
    }

    private const double Epsilon = 1e-8;

    public int BranchAndBound(Solver solver, int[] result)
    {
        var variables = solver.variables();
        var branchVars = new Variable[n * k];
        for (int i = 0; i < branchVars.Length; i++)
            branchVars[i] = variables[i];
        return BranchAndBound(solver, branchVars, result);
    }
    
    /// <summary>
    /// Perform branch and bound on initialized solver
    /// </summary>
    /// <param name="branchVars">Variables to branch on. Fewer is more efficient</param>
    /// <param name="result">Will contain the result of all variables</param>
    /// <returns>Best objective value</returns>
    public int BranchAndBound(Solver solver, Variable[] branchVars, int[] result)
    {
        var nodesStack = new Stack<BranchAndBoundNode>();
        double bestObjValue = double.NegativeInfinity;
        BranchAndBoundNode? bestNode = null;

        var node = CreateNode(solver, branchVars, new List<LinearConstraint>());
        Assert.IsTrue(node.HasValue);
        nodesStack.Push(node!.Value);

        while (nodesStack.Count > 0)
        {
            var curNode = nodesStack.Pop();
            //we computed solution for current LP earlier
            if (curNode.objectiveValue < bestObjValue + (1 - Epsilon))//no need to explore this branch
                continue;

            /*if (curNode.branchVarIndex != -1)
            {
                int varI = curNode.branchVarIndex;
                if (varI <= n * k)
                    Console.WriteLine("Branching on x " + (varI / k) + " " + (varI % k));
                else
                {
                    int zIndex = varI - (n * k);
                    Console.WriteLine("Branching on z " + (zIndex / n) + " " + (zIndex % n));
                }
            }*/

            // All variables have been set to integers
            if (curNode.branchVarIndex == -1)
            {
                Assert.IsTrue(curNode.objectiveValue >= bestObjValue + (1 - Epsilon));
                bestObjValue = curNode.objectiveValue;
                bestNode = curNode;

                //at fathomed node
                continue;
            }

            AddConstraints(solver, curNode.extraConstraints);

            var nonInt = branchVars[curNode.branchVarIndex];
            double nonIntVal = curNode.branchVarVal;
            
            //Solve L_1
            var constraint1 = nonInt <= Math.Floor(nonIntVal);
            solver.Add(constraint1);
            var extraConstraints1 = new List<LinearConstraint>(curNode.extraConstraints);
            extraConstraints1.Add(constraint1);
            var node1 = CreateNode(solver, branchVars, extraConstraints1);
            
            PopConstraints(solver, 1);

            //Solve L_2
            var constraint2 = nonInt >= Math.Ceiling(nonIntVal);
            solver.Add(constraint2);
            var extraConstraints2 = new List<LinearConstraint>(curNode.extraConstraints);
            extraConstraints2.Add(constraint2);
            var node2 = CreateNode(solver, branchVars, extraConstraints2);
            
            PopConstraints(solver, 1);
            
            PopConstraints(solver, curNode.extraConstraints.Count);
            
            if (!node1.HasValue && !node2.HasValue)
                continue;

            if (!node1.HasValue)
                nodesStack.Push(node2!.Value);
            else if (!node2.HasValue)
                nodesStack.Push(node1.Value);
            else if (node1.Value.objectiveValue > 
                     node2.Value.objectiveValue)
            {
                nodesStack.Push(node2.Value);
                nodesStack.Push(node1.Value);
            }
            else
            {
                nodesStack.Push(node1.Value);
                nodesStack.Push(node2.Value);
            }
        }

        AddConstraints(solver, bestNode.Value.extraConstraints);
        Solve(solver);
        var variables = solver.variables();
        for (int i = 0; i < variables.Count; i++)
            result[i] = (int)Math.Round(variables[i].SolutionValue());
        PopConstraints(solver, bestNode.Value.extraConstraints.Count);

        var x = ArrayUtils.Create<int>(n, k);
        GetX(result, x);
        var z = ArrayUtils.Create<int>(n, n);
        GetZ(result, z);
        for (int i = 0; i < n; i++)
        {
            for (int m = 0; m < k; m++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        continue;
                    if (x[i][m] == 1 && x[j][m] == 1)
                        Assert.IsTrue(z[i][j] == 1 && z[j][i] == 1);
                }
            }
        }
        return (int)Math.Round(bestObjValue);
    }

    public void VariableResultsToGroups(int[] variableResults, int[][] result)
    {
        int groupSize = n / k;
        for (int m = 0; m < k; m++)
        {
            var group = result[m];
            int curSize = 0;
            for (int i = 0; i < n; i++)
            {
                if (variableResults[m + (i * k)] == 1)
                    group[curSize++] = i;
            }   
            Assert.IsTrue(curSize == groupSize);  
        }
    }

    public struct BranchAndBoundNode
    {
        public List<LinearConstraint> extraConstraints;
        public double objectiveValue;
        public int branchVarIndex;
        public double branchVarVal;

        public BranchAndBoundNode(List<LinearConstraint> extraConstraints,
            double objectiveValue, int branchVarIndex, double branchVarVal)
        {
            this.extraConstraints = extraConstraints;
            this.objectiveValue = objectiveValue;
            this.branchVarIndex = branchVarIndex;
            this.branchVarVal = branchVarVal;
        }
    }
}