using System.Diagnostics;
using System.Globalization;
using System.Security.AccessControl;
using CsvHelper;
using CsvHelper.Configuration;
using Google.OrTools.LinearSolver;
using Optimization_Roommates_Problem_CS.HillClimbing;
using Optimization_Roommates_Problem_CS.Objectives;
using Optimization_Roommates_Problem_CS.SimAn;
using Optimization_Roommates_Problem_CS.SimAn.CoolingSchedules;
using Optimization_Roommates_Problem_CS.SimAn.RepetitionSchedules;
using Optimization_Roommates_Problem_CS.StopCriteria;
using Optimization_Roommates_Problem_CS.Utils;

namespace Optimization_Roommates_Problem_CS;

public static class Experiments
{
    private const int NumRuns = 5;
    private const int MaxN = 200;
    // Do not compute branch and bound for instances that
    // branches on more than MaxNumBranchVars (x_ik variables)
    private const int MaxNumBranchVars = 40;
    
    public static void Experiment1()
    {
        Experiment1(new UtilObjective());
        Experiment1(new DeonObjective());
    }
    
    private static void Experiment1(IObjective objective)
    {
        var tempOutputs = Array.Empty<ExperimentOutput>();
        int numK = 3;
        var data = CreateData(numK);
        var initStates = new int[NumRuns][][];
        // Warmup
        Experiment1(objective, 4, initStates, data, ref tempOutputs);
        ClearData(data);
        for (int n = 8; n <= MaxN; n++)
        {
            Experiment1(objective, n, initStates, data, ref tempOutputs);
        }
        string prefix = "Experiment1";
        if (objective is UtilObjective)
            prefix += "Util";
        else if (objective is DeonObjective)
            prefix += "Deon";
        WriteData(data, prefix);
    }

    private static void Experiment1(IObjective objective, int n, int[][][] initStates, 
        List<ExperimentCsvRow>[][] data, ref ExperimentOutput[] tempOutputs)
    {
        var ks = new int[] { 2, n / 4, n / 2 };
        for (int i = 0; i < ks.Length; i++)
        {
            int k = ks[i];
            if (n % k != 0 || k >= n || k < 2)
                continue;
            for (int j = 0; j < NumRuns; j++)
                initStates[j] = RoommatesUtils.RandomGroups(n, k);
            var weights = RoommatesUtils.RandomWeights(n);
            PerformExperiment(objective, initStates, weights, data[i], ref tempOutputs);
        }
    }
    
    public static void Experiment2()
    {
        var objective = new DeonObjective();
        var tempOutputs = Array.Empty<ExperimentOutput>();
        int numK = 1;
        var data = CreateData(numK);
        var initStates = new int[NumRuns][][];
        // Warmup
        Experiment2(objective, 10, initStates, data, ref tempOutputs);
        ClearData(data);
        for (int n = 10; n <= MaxN; n++)
        {
            Experiment2(objective, n, initStates, data, ref tempOutputs);
        }
        string prefix = "Experiment2";
        WriteData(data, prefix);
    }

    private static void Experiment2(IObjective objective, int n, int[][][] initStates,
        List<ExperimentCsvRow>[][] data, ref ExperimentOutput[] tempOutputs)
    {
        var ks = new int[] { n / 5 };
        for (int i = 0; i < ks.Length; i++)
        {
            int k = ks[i];
            if (n % k != 0 || k < 2)
                continue;
            for (int j = 0; j < NumRuns; j++)
                initStates[j] = RoommatesUtils.RandomGroups(n, k);
            var playerToLeague = new int[n];
            for (int j = 0; j < n; j++)
                playerToLeague[j] = RandUtils.Random.Next(1, 5 + 1);
            var weights = ArrayUtils.Create<int>(n, n);
            for (int l = 0; l < n; l++)
                for (int m = 0; m < n; m++)
                    weights[l][m] = Math.Abs(playerToLeague[l] - playerToLeague[m]);
            PerformExperiment(objective, initStates, weights, data[i], ref tempOutputs);
        }
    }
    
    public static void Experiment3()
    {
        var objective = new UtilObjective();
        var tempOutputs = Array.Empty<ExperimentOutput>();
        int numK = 1;
        var data = CreateData(numK);
        var baseWeights = ArrayUtils.Create<int>(6, 6);
        var baseGroups = new int[][]
        {
            new[] { 0, 1 }, new[] { 2, 3 }, new[] { 4, 5 }
        };
        var initStates = new int[NumRuns][][];
        // Warmup
        Experiment3(objective, 6, initStates, data, ref tempOutputs, baseGroups, baseWeights);
        ClearData(data);
        for (int n = 6; n <= MaxN; n += 6)
        {
            Experiment3(objective, n, initStates, data, ref tempOutputs, baseGroups, baseWeights);
        }
        string prefix = "Experiment3";
        WriteData(data, prefix);
    }

    private static void Experiment3(IObjective objective, int n, int[][][] initStates,
        List<ExperimentCsvRow>[][] data, ref ExperimentOutput[] tempOutputs, int[][] baseGroups, int[][] baseWeights)
    {
        for (int i = 0; i < baseWeights.Length; i++)
            for (int j = 0; j < baseWeights[i].Length; j++)
                baseWeights[i][j] = n;
        baseWeights[0][1] = baseWeights[1][0] = n / 2;
        baseWeights[0][2] = baseWeights[2][0] = 1;
        baseWeights[1][4] = baseWeights[4][1] = 1;
        baseWeights[2][3] = baseWeights[3][2] = n / 2;
        baseWeights[3][5] = baseWeights[5][3] = 1;
        baseWeights[4][5] = baseWeights[5][4] = n / 2;
        var ks = new int[] { n / 2 };
        for (int i = 0; i < ks.Length; i++)
        {
            int k = ks[i];
            if (n % k != 0 || k < 2)
                continue;
            var groups = ArrayUtils.Create<int>(k, n / k);
            var playerToLeague = new int[n];
            for (int j = 0; j < n; j++)
                playerToLeague[j] = RandUtils.Random.Next(1, 5 + 1);
            var weights = ArrayUtils.Create<int>(n, n);
            int nCubed = n * n * n;
            for (int j = 0; j < n; j++)
                for (int l = 0; l < n; l++)
                    weights[j][l] = nCubed;
            int numCliques = n / 6;
            for (int j = 0; j < numCliques; j++)
            {
                int start = j * 6;
                int groupStart = j * baseGroups.Length;
                for (int l = 0; l < baseGroups.Length; l++)
                    for (int m = 0; m < 2; m++)
                        groups[groupStart + l][m] = start + baseGroups[l][m];
                for (int l = 0; l < 6; l++)
                    for (int m = 0; m < 6; m++)
                        weights[start + l][start + m] = baseWeights[l][m];
            }
            for (int j = 0; j < NumRuns; j++)
                initStates[j] = groups;
            // Uses weights larger than n, so do not perform branch and bound
            PerformExperiment(objective, initStates, weights, data[i], ref tempOutputs, false);
        }
    }

    private static List<ExperimentCsvRow>[][] CreateData(int numK)
    {
        var data = new List<ExperimentCsvRow>[numK][];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new List<ExperimentCsvRow>[Enum.GetValues<TestFileDataType>().Length];
            for (int j = 0; j < data[i].Length; j++)
                data[i][j] = new List<ExperimentCsvRow>();
        }
        return data;
    }

    private static void ClearData(List<ExperimentCsvRow>[][] data)
    {
        for (int i = 0; i < data.Length; i++)
            for (int j = 0; j < data[i].Length; j++)
                data[i][j].Clear();
    }

    private static void WriteData(List<ExperimentCsvRow>[][] data, string filePrefix)
    {
        for (int i = 0; i < data.Length; i++)
        {
            string finalPrefix = filePrefix + "k" + i;
            for (int j = 0; j < data[i].Length; j++)
            {
                using var writer = new StreamWriter(
                    finalPrefix + (TestFileDataType)j + ".csv");
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                var rows = data[i][j];
                csv.WriteHeader<ExperimentCsvRow>();
                csv.NextRecord();
                for (int k = 0; k < rows.Count; k++)
                {
                    csv.WriteRecord(rows[k]);
                    csv.NextRecord();
                } 
            }
        }
    }
    
    public static void PerformExperiment(IObjective objective, int[][][] initStates,
        int[][] weights, List<ExperimentCsvRow>[] data, 
        ref ExperimentOutput[] tempOutputs, bool includeBranchAndBound = true)
    {
        int numRuns = initStates.Length;
        ArrayUtils.TryDoubleOrSetSize(ref tempOutputs, numRuns);
        for (int i = 0; i < numRuns; i++)
        {
            if (tempOutputs[i] == null)
                tempOutputs[i] = new ExperimentOutput();
            else
                tempOutputs[i].Reset();
        }
        int n = weights.Length;
        int k = initStates[0].Length;
        Console.WriteLine("Working on " + n + " " + k);
        for (int i = 0; i < data.Length; i++)
            data[i].Add(new ExperimentCsvRow() { n = n });
        var sw = new Stopwatch();
        var simple = new SimpleHillClimber();
        ExperimentHillClimber(sw, simple, objective, initStates, weights, 
            Algorithm.SimpleHc, data, ref tempOutputs);
        
        var steepest = new SteepestAscentHillClimber();
        ExperimentHillClimber(sw, steepest, objective, initStates, weights, 
            Algorithm.SteepestHc, data, ref tempOutputs);
        
        var firstChoice = new FirstChoiceHillClimber();
        //RoommatesUtils.GreedyGroups(objective, groups, weights);
        ExperimentHillClimber(sw, firstChoice, objective, initStates, weights, 
            Algorithm.FirstChoiceHc, data, ref tempOutputs);
        
        double prob = 0.9;
        // solve(1 / (1 + exp(-d/t)) = prob, t)
        // Find time such that probability of choosing MaxDeltaValue is 0.9
        double t = objective.MaxDeltaValue(n, k) / Math.Log(-(prob - 1) / prob);
        var stochastic = new StochasticHillClimber(t);
        ExperimentHillClimber(sw, stochastic, objective, initStates, weights, 
            Algorithm.StochasticHc, data, ref tempOutputs);
        
        var stopCriterion = new MaxNumStepsStopCriterion(int.MaxValue);
        int maxNumRestarts = 10;
        var randomRestart = new RandomRestartHillClimber(maxNumRestarts);
        var innerHc = new FirstChoiceHillClimber();
        for (int i = 0; i < numRuns; i++)
        {
            tempOutputs[i].Reset();
            sw.Restart();
            randomRestart.Initialize(ArrayUtils.Clone(initStates[i]), weights, objective);
            while (randomRestart.TryRestart(innerHc, stopCriterion))
            {
                ExperimentHillClimber(sw, randomRestart, objective, 
                    randomRestart.GetGroups(), weights, ref tempOutputs[i], reset: false);
            }
            tempOutputs[i].Value = randomRestart.BestValue;
        }
        SetRowsFromAvg(data, Algorithm.RandomRestartHc, tempOutputs, numRuns);
        
        for (int i = 0; i < numRuns; i++)
        {
            tempOutputs[i].Reset();
            sw.Restart();
            //RoommatesUtils.GreedyGroups(objective, groups, weights);
            double initProb = 0.4f;
            double initTemp = ParamSelector.CalcBenAmeurInitialTemperature(objective, initStates[i], weights, 
                new List<Transition>(), initProb);
            int expectNumNeighbors = objective.ExpectedNeighborhoodSize(n, k);
            int sizeFactor = 16;
            var simAn = new SimulatedAnnealing(new GeometricCoolingSchedule(0.95), initTemp, 
                new ConstantRepetitionSchedule(expectNumNeighbors * sizeFactor), 
                new MinPercentStopCriterion());
            simAn.Initialize(ArrayUtils.Clone(initStates[i]), weights, objective);
            ExperimentHillClimber(sw, simAn, objective, 
                simAn.GetGroups(), weights, ref tempOutputs[i], reset: false);
        }
        SetRowsFromAvg(data, Algorithm.SimAn, tempOutputs, numRuns);

        if (!includeBranchAndBound || n * k > MaxNumBranchVars)
            return;
        // Only 1 run needed for branch and bound
        numRuns = 1;
        for (int i = 0; i < numRuns; i++)
        {
            var tempOutput = tempOutputs[i];
            tempOutput.Reset();
            sw.Restart();
            var roommatesLpSolver = new RoommatesLpSolver();
            Solver solver;
            if (objective is UtilObjective)
                solver = roommatesLpSolver.InitializeUtil(k, weights);
            else if (objective is DeonObjective)
                solver = roommatesLpSolver.InitializeDeon(k, weights);
            else
            {
                throw new ArgumentException("Objective " + objective +
                                            " is not supported by the linear solver");   
            }
            var result = new int[solver.variables().Count];
            roommatesLpSolver.BranchAndBound(solver, result);
            sw.Stop();
            var groupsBb = ArrayUtils.Create<int>(k, n / k);
            roommatesLpSolver.VariableResultsToGroups(result, groupsBb);
            tempOutput.TimeSeconds = sw.Elapsed.TotalSeconds;
            tempOutput.Value = objective.CalculateSlow(groupsBb, weights);
        }
        SetRowsFromAvg(data, Algorithm.BranchAndBound, tempOutputs, numRuns);
    }

    private static void ExperimentHillClimber(Stopwatch sw, BaseHillClimber hc,
        IObjective objective, int[][][] initStates, int[][] weights, 
        Algorithm algorithm, List<ExperimentCsvRow>[] data, ref ExperimentOutput[] tempOutputs)
    {
        int numRuns = initStates.Length;
        for (int i = 0; i < numRuns; i++)
        {
            ExperimentHillClimber(sw, hc, objective,
                initStates[i], weights, ref tempOutputs[i]);
        }
        SetRowsFromAvg(data, algorithm, tempOutputs, numRuns);
    }

    private static void ExperimentHillClimber(Stopwatch sw, BaseHillClimber hc,
        IObjective objective, int[][] groups, int[][] weights, 
        ref ExperimentOutput output, bool reset = true)
    {
        //RoommatesUtils.GreedyGroups(objective, groups, weights);
        if (reset)
        {
            output.Reset();
            sw.Restart();
            hc.Initialize(ArrayUtils.Clone(groups), weights, objective);
        } 
        else 
            sw.Start();
        long numNeighborsTotal = 0;
        double prevValue = hc.Value;
        double swapDelta = 0;
        while (hc.NextStep())
        {
            numNeighborsTotal += hc.NumNeighbors;
            swapDelta += hc.Value - prevValue;
            prevValue = hc.Value;
        }
        sw.Stop();
        output.SwapDelta = NumericUtils.CombineMeans(output.SwapDelta, output.NumSwaps,
            hc.NumSteps == 0 ? 0 : swapDelta / hc.NumSteps, hc.NumSteps);
        output.NumNeighborsPerSwap = NumericUtils.CombineMeans(
            output.NumNeighborsPerSwap, output.NumSwaps, 
            hc.NumSteps == 0 ? 0 : numNeighborsTotal / (double)hc.NumSteps, hc.NumSteps);

        output.TimeSeconds += sw.Elapsed.TotalSeconds;
        output.NumSwaps += hc.NumSteps;
        output.Value += hc.Value;
    }
    
    private static void SetRowsFromAvg(List<ExperimentCsvRow>[] data, 
        Algorithm algorithm, ExperimentOutput[] outputs, int numOutputs)
    {
        var usedOutputs = outputs.Take(numOutputs);
        double timeSeconds = usedOutputs.Average(o => o.TimeSeconds);
        double timeSecondsStd = usedOutputs.Select(o => o.TimeSeconds).StandardDeviation();
        double numSwaps = usedOutputs.Average(o => o.NumSwaps);
        double numSwapsStd = usedOutputs.Select(o => (double)o.NumSwaps).StandardDeviation();
        double swapDelta = usedOutputs.Average(o => o.SwapDelta);
        if (double.IsNaN(swapDelta))
            throw new Exception();
        double swapDeltaStd = usedOutputs.Select(o => o.SwapDelta).StandardDeviation();
        double numNeighborsPerSwap = usedOutputs.Average(o => o.NumNeighborsPerSwap);
        double numNeighborsPerSwapStd = 
            usedOutputs.Select(o => o.NumNeighborsPerSwap).StandardDeviation();
        double value = usedOutputs.Average(o => o.Value);
        double valueStd = usedOutputs.Select(o => o.Value).StandardDeviation();
        int rowIndex = data[0].Count - 1;
        SetCsvRowForAlgorithm(data[(int)TestFileDataType.Time][rowIndex],
            algorithm, timeSeconds, timeSecondsStd);
        SetCsvRowForAlgorithm(data[(int)TestFileDataType.Swaps][rowIndex],
            algorithm, numSwaps, numSwapsStd);
        SetCsvRowForAlgorithm(data[(int)TestFileDataType.SwapDelta][rowIndex],
            algorithm, swapDelta, swapDeltaStd);
        SetCsvRowForAlgorithm(data[(int)TestFileDataType.NeighborsPerSwap][rowIndex],
            algorithm, numNeighborsPerSwap, numNeighborsPerSwapStd);
        SetCsvRowForAlgorithm(data[(int)TestFileDataType.Value][rowIndex],
            algorithm, value, valueStd);
    }

    private static void SetCsvRowForAlgorithm(ExperimentCsvRow csvRow, 
        Algorithm algorithm, double avg, double std)
    {
        switch (algorithm)
        {
        case Algorithm.SimpleHc:
            csvRow.SimpleHc = avg;
            csvRow.SimpleHcStd = std;
            break;
        case Algorithm.SteepestHc:
            csvRow.SteepestHc = avg;
            csvRow.SteepestHcStd = std;
            break;
        case Algorithm.FirstChoiceHc:
            csvRow.FirstChoiceHc = avg;
            csvRow.FirstChoiceHcStd = std;
            break;
        case Algorithm.StochasticHc:
            csvRow.StochasticHc = avg;
            csvRow.StochasticHcStd = std;
            break;
        case Algorithm.RandomRestartHc:
            csvRow.RandomRestart = avg;
            csvRow.RandomRestartStd = std;
            break;
        case Algorithm.SimAn:
            csvRow.SimAn = avg;
            csvRow.SimAnStd = std;
            break;
        case Algorithm.BranchAndBound:
            csvRow.BranchAndBound = avg;
            csvRow.BranchAndBoundStd = std;
            break;
        }
    }

    /*private static ExperimentDataRow CreateRowFromAvg(int n,
        ExperimentOutput[] outputs, int numOutputs)
    {
        var row = new ExperimentDataRow() { n = n };
        var usedOutputs = outputs.Take(numOutputs);
        row.TimeSeconds = usedOutputs.Average(o => o.TimeSeconds);
        row.TimeSecondsStd = usedOutputs.Select(o => o.TimeSeconds).StandardDeviation();
        row.NumSwaps = usedOutputs.Average(o => o.NumSwaps);
        row.NumSwapsStd = usedOutputs.Select(o => (double)o.NumSwaps).StandardDeviation();
        row.SwapDelta = usedOutputs.Average(o => o.SwapDelta);
        row.SwapDeltaStd = usedOutputs.Select(o => o.SwapDelta).StandardDeviation();
        row.NumNeighborsPerSwap = usedOutputs.Average(o => o.NumNeighborsPerSwap);
        row.NumNeighborsPerSwapStd = 
            usedOutputs.Select(o => o.NumNeighborsPerSwap).StandardDeviation();
        row.Value = usedOutputs.Average(o => o.Value);
        row.ValueStd = usedOutputs.Select(o => o.Value).StandardDeviation();
        return row;
    }*/
}

public class ExperimentOutput
{
    public double TimeSeconds;
    public int NumSwaps;
    public double SwapDelta;
    public double NumNeighborsPerSwap;
    public double Value;

    public void Reset()
    {
        TimeSeconds = SwapDelta = NumNeighborsPerSwap = Value = 0;
        NumSwaps = 0;
    }
}

public class ExperimentDataRow
{
    public int n;
    public double TimeSeconds;
    public double TimeSecondsStd;
    public double NumSwaps;
    public double NumSwapsStd;
    public double SwapDelta;
    public double SwapDeltaStd;
    public double NumNeighborsPerSwap;
    public double NumNeighborsPerSwapStd;
    public double Value;
    public double ValueStd;
}

public class ExperimentCsvRow
{
    public int n { get; set; }
    public double SimpleHc { get; set; } = -1;
    public double SimpleHcStd { get; set; } = -1;
    public double SteepestHc { get; set; } = -1;
    public double SteepestHcStd { get; set; } = -1;
    public double FirstChoiceHc { get; set; } = -1;
    public double FirstChoiceHcStd { get; set; } = -1;
    public double StochasticHc { get; set; } = -1;
    public double StochasticHcStd { get; set; } = -1;
    public double RandomRestart { get; set; } = -1;
    public double RandomRestartStd { get; set; } = -1;
    public double SimAn { get; set; } = -1;
    public double SimAnStd { get; set; } = -1;
    public double BranchAndBound { get; set; } = -1;
    public double BranchAndBoundStd { get; set; } = -1;
}

public enum TestFileDataType
{
    Time,
    Value,
    Swaps,
    SwapDelta,
    NeighborsPerSwap
}

public enum Algorithm
{
    SimpleHc,
    SteepestHc,
    FirstChoiceHc,
    StochasticHc,
    RandomRestartHc,
    SimAn,
    BranchAndBound
}
