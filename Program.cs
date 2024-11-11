
using Gurobi;
using Google.OrTools.LinearSolver;
using Google.OrTools.Sat;
using System.Runtime.InteropServices;
using System.Diagnostics;

/// <summary>
/// 不同线之间无法换乘，可以排除
/// </summary>


class BusScheduleOptimization
{
    static void Main()
    {
        // 小算例
        int[] timeInterval = [10, 10, 10, 20]; // 发车时间间隔
        int[][] timeRunning = [[5, 12, 10000, 10000], [10000, 10000, 10, 4], [6, 10000, 10000, 9], [10000, 5, 13, 10000]]; // 从始发站到换乘站的行驶时间
        int[] timeTransfer = [0, 0, 0, 0]; // 换乘时间
        int totalTimeInterval = 30; // 总时间
        int timeLimit = 10000; // 时间限制
        double solverTimeLimit = 30.0; // 求解器时间限制

        // 大算例
        //int[] timeInterval = [10, 10, 15, 25, 20, 11, 15, 10, 14, 17, 24]; // 发车时间间隔
        //int[][] timeRunning = [[10000, 10000, 19, 24, 10000, 32, 10000, 36, 10000, 45, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000],
        //                      [10000, 10000, 42, 37, 10000, 29, 10000, 25, 23, 10000, 1000, 10000, 10000, 10000, 10000, 10000, 10000, 10000],
        //                      [10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 18, 42, 58, 10000, 10000, 10000, 10000, 51],
        //                      [10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 90, 72, 10000, 14, 26, 22, 10000, 49, 10000],
        //                      [10000, 17, 22, 10000, 34, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000],
        //                      [37, 17, 26, 31, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000],
        //                      [10000, 10000, 10000, 10000, 61, 53, 51, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000],
        //                      [10000, 10000, 10000, 10000, 10000, 10000, 55, 10000, 48, 10000, 21, 10000, 10000, 10000, 10000, 10000, 10000, 10000],
        //                      [10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 16, 20, 32, 10000, 10000],
        //                      [10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 20, 10000, 10000],
        //                      [10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 20, 28, 10000, 10000, 10000, 10000, 10000]];
        //int[] timeTransfer = [4, 3, 2, 6, 0, 2, 7, 0, 6, 0, 1, 1, 0, 0, 1, 12, 11, 3]; // 换乘时间
        //int totalTimeInterval = 240;
        //int timeLimit = 10000;
        //double solverTimeLimit = 600.0;

        BusSchedule busSchedule = new(timeInterval, timeRunning, timeTransfer, totalTimeInterval, timeLimit, solverTimeLimit);

        // 换乘客流transferPassengerFlow[i,k,m],m站点i线路换乘j线路
        double[,,] transferPassengerFlow = new double[busSchedule.timeInterval.Length, busSchedule.timeInterval.Length, busSchedule.timeRunning[0].Length];

        // 大案例
        //transferPassengerFlow[5, 4, 0] = 1.2;//S1 B2-B1
        //transferPassengerFlow[5, 4, 1] = 0.4;//S2 B2-B1
        //transferPassengerFlow[4, 5, 1] = 0.4;//S2 B1-B2
        //transferPassengerFlow[5, 4, 2] = 0.4;//S3 B2-B1
        //transferPassengerFlow[4, 5, 2] = 0.4;//S3 B1-B2
        //transferPassengerFlow[0, 4, 2] = 0.4;//S3 A1-B1
        //transferPassengerFlow[0, 5, 2] = 0.4;//S3 A1-B2
        //transferPassengerFlow[4, 0, 2] = 0.8;//S3 B1-A1
        //transferPassengerFlow[4, 1, 2] = 0.8;//S3 B1-A2
        //transferPassengerFlow[5, 0, 2] = 0.8;//S3 B2-A1
        //transferPassengerFlow[5, 1, 2] = 0.8;//S3 B2-A2
        //transferPassengerFlow[1, 5, 3] = 0.8;//S4 A2-B2
        //transferPassengerFlow[6, 4, 4] = 0.4;//S5 B3-B1
        //transferPassengerFlow[6, 0, 5] = 0.4;//S6 B3-A1
        //transferPassengerFlow[6, 1, 5] = 0.8;//S6 B3-A2
        //transferPassengerFlow[7, 6, 6] = 0.4;//S7 B4-B3
        //transferPassengerFlow[1, 0, 7] = 6;//S8 A2-A1
        //transferPassengerFlow[1, 7, 8] = 0.4;//S9 A2-B4
        //transferPassengerFlow[0, 2, 9] = 12;//S10 A1-A3
        //transferPassengerFlow[7, 2, 10] = 0.4;//S11 B4-A3
        //transferPassengerFlow[3, 7, 10] = 1;//S11 A4-B4
        //transferPassengerFlow[2, 10, 11] = 0.4;//S12 A3-B7
        //transferPassengerFlow[10, 2, 11] = 0.8;//S12 B7-A3
        //transferPassengerFlow[10, 9, 12] = 1.2;//S13 B7-B6
        //transferPassengerFlow[10, 3, 12] = 1.6; //S13 B7-A4
        //transferPassengerFlow[2, 9, 12] = 0.8;//S13 A3-B6
        //transferPassengerFlow[8, 3, 13] = 0.4;//S14 B5-A4
        //transferPassengerFlow[3, 8, 14] = 1.2;//S15 A4-B5
        //transferPassengerFlow[8, 9, 15] = 0.4;//S16 B5-B6
        //transferPassengerFlow[3, 10, 16] = 2.4;//S17 A4-B7
        //transferPassengerFlow[2, 8, 17] = 0.8;//S18 A3-B5
        //for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        //{
        //    for (int j = 0; j < busSchedule.timeInterval.Length; j++)
        //    {
        //        for (int k = 0; k < busSchedule.timeRunning[0].Length; k++)
        //        {
        //            transferPassengerFlow[i, j, k] = 10 * transferPassengerFlow[i, j, k];
        //        }
        //    }
        //}

        // 小案例
        transferPassengerFlow[0, 2, 0] = 12;//S1 I-III
        transferPassengerFlow[2, 0, 0] = 8;//S1  III-I
        transferPassengerFlow[0, 3, 1] = 8;//S2  I-IV
        transferPassengerFlow[3, 0, 1] = 8;//S2  IV-I
        transferPassengerFlow[1, 3, 2] = 10;//S3 II-IV
        transferPassengerFlow[3, 1, 2] = 8;//S3  IV-II
        transferPassengerFlow[1, 2, 3] = 7;//S4  II-III
        transferPassengerFlow[2, 1, 3] = 11;//S4 III-II

        // Model1
        // Optimization(busSchedule);
        // OptimizationSCIP(busSchedule);
        OptimizationCP_SAT(busSchedule);
        // OptimizationOrtools(busSchedule);

        // Model2
        double p = OptimizationCP_SAT_Model2(busSchedule, transferPassengerFlow);

        // Model3
        OptimizationCP_SAT_Model3(busSchedule, transferPassengerFlow, p);
        OptimizationGurobi_Model3(busSchedule, transferPassengerFlow, p);

        // Model4
        OptimizationCP_SAT_Model4(busSchedule, transferPassengerFlow, p);
        OptimizationGurobi_Model4(busSchedule, transferPassengerFlow, p);

    }

    private static void RunScipSolver(string scipPath, string modelPath, double timeLimit)
    {
        // 设置时间限制并指定输出解的文件
        string solutionFilePath = "solution.sol";
        string arguments = $"-c \"read {modelPath}\" -c \"set limits time {timeLimit}\" -c \"optimize\" -c \"write solution {solutionFilePath}\" -c \"quit\"";

        // 创建进程以运行 SCIP 命令行
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = scipPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process())
        {
            process.StartInfo = processStartInfo;
            process.Start();

            // 读取输出和错误
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            // 输出 SCIP 的求解结果
            Console.WriteLine("SCIP output: ");
            Console.WriteLine(output);

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("SCIP error: ");
                Console.WriteLine(error);
            }
        }

        // 读取解文件并解析结果
        if (File.Exists(solutionFilePath))
        {
            ParseSolution(solutionFilePath);
        }
        else
        {
            Console.WriteLine("Solution file not found.");
        }
    }

    // 解析最佳目标函数值
    public static void ParseSolution(string solutionFilePath)
    {
        Console.WriteLine("Parsing solution...");

        string[] solutionLines = File.ReadAllLines(solutionFilePath);
        double objectiveValue = double.NaN;
        var variableValues = new Dictionary<string, double>();

        foreach (string line in solutionLines)
        {
            // 查找目标函数值，行格式类似 "objective value: 1024"
            if (line.Trim().StartsWith("objective value"))
            {
                string[] parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 && double.TryParse(parts[1].Trim(), out objectiveValue))
                {
                    Console.WriteLine($"Objective Value: {objectiveValue}");
                }
            }
            // 解析变量及其对应的值
            else if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#") && !line.Contains("solution status"))
            {
                string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 2)
                {
                    string variableName = parts[0]; // 变量名
                    if (double.TryParse(parts[1], out double variableValue))
                    {
                        variableValues[variableName] = variableValue;
                        Console.WriteLine($"Variable: {variableName}, Value: {variableValue}");
                    }
                }
            }
        }
    }


    private static void OptimizationOrtools(BusSchedule busSchedule)
    {
        // Create the solver
        Solver solver = Solver.CreateSolver("CBC_MIXED_INTEGER_PROGRAMMING");

        // 设置求解时间上限
        solver.SetTimeLimit(busSchedule.timeLimit);  // 单位毫秒

        int M = 100000;
        int[] carCount = new int[busSchedule.timeInterval.Length];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            carCount[i] = (int)Math.Ceiling((double)busSchedule.totalTimeInterval / (double)busSchedule.timeInterval[i]);
        }

        // Decision variables x[i, j] (integer variables)
        Variable[,] x = new Variable[busSchedule.timeInterval.Length, carCount.Max()];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                // x[i, j] = solver.MakeIntVar(0, busSchedule.totalTimeInterval, $"x{i}_{j}");
                if ((j + 1) * busSchedule.timeInterval[i] >= busSchedule.totalTimeInterval)
                {
                    x[i, j] = solver.MakeIntVar(j * busSchedule.timeInterval[i], busSchedule.totalTimeInterval, $"x{i}_{j}");
                }
                else
                {
                    x[i, j] = solver.MakeIntVar(j * busSchedule.timeInterval[i], (j + 1) * busSchedule.timeInterval[i], $"x{i}_{j}");
                }
            }
        }

        // Decision variables y[i, j, k, l, m] (binary variables)
        Variable[,,,,] y = new Variable[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
        Objective objective = solver.Objective();
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                {
                    for (int l = 0; l < carCount[k]; l++)
                    {
                        for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                        {
                            y[i, j, k, l, m] = solver.MakeBoolVar($"y{i}_{j}_{k}_{l}_{m}");
                            if (i != k)
                            {
                                objective.SetCoefficient(y[i, j, k, l, m], 1);
                            }
                        }
                    }
                }
            }
        }

        // Constraints for y variables
        // Fixing constraint with sum of boolean variables
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int k = 0; k < busSchedule.timeInterval.Length; k++)
            {
                for (int l = 0; l < carCount[k]; l++)
                {
                    for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                    {
                        if (busSchedule.timeRunning[i][m] == busSchedule.timeLimit || busSchedule.timeRunning[k][m] == busSchedule.timeLimit)
                        {
                            for (int j = 0; j < carCount[i]; j++)
                            {
                                solver.Add(y[i, j, k, l, m] == 0);
                            }
                        }

                        Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(double.NegativeInfinity, 10, $"constraint1:{i} {k} {l} {m}");

                        for (int j = 0; j < carCount[i]; j++)
                        {
                            if (j == 0)
                            {
                                solver.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                solver.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeRunning[i][m] - 1 + M * (1 - y[i, j, k, l, m]));
                            }
                            else
                            {
                                solver.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                solver.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeInterval[i] - 1 + M * (1 - y[i, j, k, l, m]));
                            }

                            constraint.SetCoefficient(y[i, j, k, l, m], 1);
                        }
                    }
                }
            }
        }


        // Constraints for x variables (separation constraint)
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i] - 1; j++)
            {
                solver.Add(x[i, j + 1] - x[i, j] == busSchedule.timeInterval[i]);
            }
        }

        // Define the objective function (maximization)
        objective.SetMaximization();

        // Solve the model
        Solver.ResultStatus resultStatus = solver.Solve();

        // output the result
        if (resultStatus == Solver.ResultStatus.OPTIMAL)
        {
            Console.WriteLine("最优解:");
            Console.WriteLine($"最大化目标函数值 = {solver.Objective().Value()}");
        }
        else
        {
            Console.WriteLine("无法找到最优解。");
        }
    }

    private static void OptimizationCP_SAT(BusSchedule busSchedule)
    {
        // Create the CP-SAT model
        CpModel model = new CpModel();

        int M = 100000;
        int[] carCount = new int[busSchedule.timeInterval.Length];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            carCount[i] = (int)Math.Ceiling((double)busSchedule.totalTimeInterval / (double)busSchedule.timeInterval[i]);
        }

        // Decision variables x[i, j] (integer variables)
        IntVar[,] x = new IntVar[busSchedule.timeInterval.Length, carCount.Max()];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                if ((j + 1) * busSchedule.timeInterval[i] >= busSchedule.totalTimeInterval)
                {
                    x[i, j] = model.NewIntVar(j * busSchedule.timeInterval[i], busSchedule.totalTimeInterval, $"x{i}_{j}");
                }
                else
                {
                    x[i, j] = model.NewIntVar(j * busSchedule.timeInterval[i], (j + 1) * busSchedule.timeInterval[i], $"x{i}_{j}");
                }
            }
        }

        // Decision variables y[i, j, k, l, m] (binary variables)
        IntVar[,,,,] y = new IntVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                {
                    for (int l = 0; l < carCount[k]; l++)
                    {
                        for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                        {
                            y[i, j, k, l, m] = model.NewBoolVar($"y{i}_{j}_{k}_{l}_{m}");
                        }
                    }
                }
            }
        }

        // Constraints for y variables
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int k = 0; k < busSchedule.timeInterval.Length; k++)
            {
                for (int l = 0; l < carCount[k]; l++)
                {
                    for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                    {
                        // situation 1: cannot transfer between two lines
                        if (busSchedule.timeRunning[i][m] == busSchedule.timeLimit || busSchedule.timeRunning[k][m] == busSchedule.timeLimit)
                        {
                            for (int j = 0; j < carCount[i]; j++)
                            {
                                model.Add(y[i, j, k, l, m] == 0);
                            }
                            continue;
                        }


                        for (int j = 0; j < carCount[i]; j++)
                        {
                            // situation 2: cannot transfer between two vehicles
                            if (i != k)
                            {
                                if (j == 0)
                                {
                                    if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) || (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeRunning[i][m] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                    {
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeRunning[i][m] + M * (1 - y[i, j, k, l, m]));
                                    }
                                    else
                                    {
                                        model.Add(y[i, j, k, l, m] == 0);
                                    }
                                }
                                else
                                {
                                    if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) || (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeInterval[i] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                    {
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeInterval[i] - 1 + M * (1 - y[i, j, k, l, m]));
                                    }
                                    else
                                    {
                                        model.Add(y[i, j, k, l, m] == 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Constraints for x variables (separation constraint)
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i] - 1; j++)
            {
                model.Add(x[i, j + 1] - x[i, j] == busSchedule.timeInterval[i]);
            }
        }

        // Define the objective function (maximization)
        Google.OrTools.Sat.LinearExpr objective = Google.OrTools.Sat.LinearExpr.Sum(
            from i in Enumerable.Range(0, busSchedule.timeInterval.Length)
            from j in Enumerable.Range(0, carCount[i])
            from k in Enumerable.Range(0, busSchedule.timeInterval.Length)
            from l in Enumerable.Range(0, carCount[k])
            from m in Enumerable.Range(0, busSchedule.timeRunning[0].Length)
            where i != k
            select y[i, j, k, l, m]
        );
        model.Maximize(objective);

        // Solve the model
        CpSolver solver = new CpSolver();
        // 设置求解时间上限
        solver.StringParameters = $"max_time_in_seconds:{busSchedule.solverTimeLimit}";
        CpSolverStatus resultStatus = solver.Solve(model);

        // Output the result
        if (resultStatus == CpSolverStatus.Optimal || resultStatus == CpSolverStatus.Feasible)
        {
            Console.WriteLine("最优解:");
            Console.WriteLine($"最大化目标函数值 = {solver.ObjectiveValue}");
        }
        else
        {
            Console.WriteLine("无法找到最优解。");
        }
    }

    private static void Optimization(BusSchedule busSchedule)
    {
        try
        {
            GRBEnv env = new();
            GRBModel model = new(env);

            int M = 100000;
            int[] carCount = new int[busSchedule.timeInterval.Length];
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                carCount[i] = (int)Math.Ceiling((double)busSchedule.totalTimeInterval / (double)busSchedule.timeInterval[i]);
            }

            GRBVar[,] x = new GRBVar[busSchedule.timeInterval.Length, carCount.Max()];
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int j = 0; j < carCount[i]; j++)
                {
                    x[i, j] = model.AddVar(0.0, busSchedule.totalTimeInterval, 0.0, GRB.INTEGER, "x" + i + j);
                }
            }

            GRBVar[,,,,] y = new GRBVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int j = 0; j < carCount[i]; j++)
                {
                    for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                    {
                        for (int l = 0; l < carCount[k]; l++)
                        {
                            for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                            {
                                if (i != k)
                                {
                                    y[i, j, k, l, m] = model.AddVar(0.0, 1.0, 1.0, GRB.BINARY, "y" + i + j + k + l + m);
                                }
                                else
                                {
                                    y[i, j, k, l, m] = model.AddVar(0.0, 0.0, 0.0, GRB.BINARY, "y" + i + j + k + l + m);
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                {
                    for (int l = 0; l < carCount[k]; l++)
                    {
                        for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                        {
                            if (busSchedule.timeRunning[i][m] == busSchedule.timeLimit || busSchedule.timeRunning[k][m] == busSchedule.timeLimit)
                            {
                                for (int j = 0; j < carCount[i]; j++)
                                {
                                    model.AddConstr(y[i, j, k, l, m] == 0, "c0" + i + j + k + l + m);
                                }
                            }
                            GRBLinExpr expr = 0d;
                            for (int j = 0; j < carCount[i]; j++)
                            {
                                expr.AddTerm(1.0, y[i, j, k, l, m]);
                                if (j == 0)
                                {
                                    model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]), "c1" + i + j + k + l + m);
                                    model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeRunning[i][m] - 1 + M * (1 - y[i, j, k, l, m]), "c2" + i + j + k + l + m);
                                }
                                else
                                {
                                    model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]), "c1" + i + j + k + l + m);
                                    model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeInterval[i] - 1 + M * (1 - y[i, j, k, l, m]), "c2" + i + j + k + l + m);
                                }
                            }
                            model.AddConstr(expr <= 1, "c3" + i + k + l + m);
                        }
                    }
                }
            }

            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int j = 0; j < carCount[i] - 1; j++)
                {
                    model.AddConstr(x[i, j + 1] - x[i, j] == busSchedule.timeInterval[i], "c4" + i + j);
                }
            }

            model.ModelSense = GRB.MAXIMIZE;
            model.Optimize();

            // 输出x变量的值
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int j = 0; j < carCount[i]; j++)
                {
                    Console.WriteLine($"x{i}{j} = {x[i, j].X}");
                }
            }
        }
        catch
        {

        }
    }

    private static void OptimizationSCIP(BusSchedule busSchedule)
    {
        // 替换 solver 为 SCIP
        Solver solver = Solver.CreateSolver("SCIP_MIXED_INTEGER_PROGRAMMING");
        if (solver == null)
        {
            Console.WriteLine("SCIP solver not found.");
            return;
        }

        solver.SolverVersion();

        int M = 100000;
        int[] carCount = new int[busSchedule.timeInterval.Length];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            carCount[i] = (int)Math.Ceiling((double)busSchedule.totalTimeInterval / (double)busSchedule.timeInterval[i]);
        }

        // Decision variables x[i, j] (integer variables)
        Variable[,] x = new Variable[busSchedule.timeInterval.Length, carCount.Max()];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                x[i, j] = solver.MakeIntVar(0, busSchedule.totalTimeInterval, $"x{i}_{j}");
                //if ((j + 1) * busSchedule.timeInterval[i] >= busSchedule.totalTimeInterval)
                //{
                //    x[i, j] = solver.MakeIntVar(j * busSchedule.timeInterval[i], busSchedule.totalTimeInterval, $"x{i}_{j}");
                //}
                //else
                //{
                //    x[i, j] = solver.MakeIntVar(j * busSchedule.timeInterval[i], (j + 1) * busSchedule.timeInterval[i], $"x{i}_{j}");
                //}
            }
        }

        // Decision variables y[i, j, k, l, m] (binary variables)
        Variable[,,,,] y = new Variable[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
        Objective objective = solver.Objective();
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                {
                    for (int l = 0; l < carCount[k]; l++)
                    {
                        for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                        {
                            y[i, j, k, l, m] = solver.MakeBoolVar($"y{i}_{j}_{k}_{l}_{m}");
                            if (i != k)
                            {
                                objective.SetCoefficient(y[i, j, k, l, m], 1);
                            }
                        }
                    }
                }
            }
        }

        // Constraints for y variables
        // Fixing constraint with sum of boolean variables
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int k = 0; k < busSchedule.timeInterval.Length; k++)
            {
                for (int l = 0; l < carCount[k]; l++)
                {
                    for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                    {
                        if (busSchedule.timeRunning[i][m] == busSchedule.timeLimit || busSchedule.timeRunning[k][m] == busSchedule.timeLimit)
                        {
                            for (int j = 0; j < carCount[i]; j++)
                            {
                                solver.Add(y[i, j, k, l, m] == 0);
                            }
                        }

                        Google.OrTools.LinearSolver.Constraint constraint = solver.MakeConstraint(double.NegativeInfinity, 10, $"constraint1:{i} {k} {l} {m}");

                        for (int j = 0; j < carCount[i]; j++)
                        {
                            if (j == 0)
                            {
                                solver.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                solver.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeRunning[i][m] - 1 + M * (1 - y[i, j, k, l, m]));
                            }
                            else
                            {
                                solver.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                solver.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeInterval[i] - 1 + M * (1 - y[i, j, k, l, m]));
                            }

                            constraint.SetCoefficient(y[i, j, k, l, m], 1);
                        }
                    }
                }
            }
        }


        // Constraints for x variables (separation constraint)
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i] - 1; j++)
            {
                solver.Add(x[i, j + 1] - x[i, j] == busSchedule.timeInterval[i]);
            }
        }

        // Define the objective function (maximization)
        objective.SetMaximization();

        // 定义导出路径
        string modelPath = "model.lp";
        if (File.Exists(modelPath))
        {
            File.Delete(modelPath);
        }

        // 使用 ExportModelAsLpFormat() 获取 LP 格式的模型字符串
        string lpModel = solver.ExportModelAsLpFormat(false);

        // 将模型字符串写入文件
        using (StreamWriter writer = new StreamWriter(modelPath))
        {
            writer.Write(lpModel);  // 写入模型内容
        }

        Console.WriteLine($"Model exported to: {modelPath}");

        // 调用 SCIP 命令行
        string scipPath = @"D:\SCIPOptSuite 9.1.1\bin\scip.exe";
        RunScipSolver(scipPath, modelPath, busSchedule.solverTimeLimit);

    }

    private static double OptimizationCP_SAT_Model2(BusSchedule busSchedule, double[,,] transferPassengerFlow)
    {
        // Create the CP-SAT model
        CpModel model = new CpModel();

        int M = 100000;
        int[] carCount = new int[busSchedule.timeInterval.Length];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            carCount[i] = (int)Math.Ceiling((double)busSchedule.totalTimeInterval / (double)busSchedule.timeInterval[i]); //向上取整
        }

        // Decision variables x[i, j] (integer variables)
        IntVar[,] x = new IntVar[busSchedule.timeInterval.Length, carCount.Max()];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                if ((j + 1) * busSchedule.timeInterval[i] >= busSchedule.totalTimeInterval)
                {
                    x[i, j] = model.NewIntVar(j * busSchedule.timeInterval[i], busSchedule.totalTimeInterval, $"x{i}_{j}");
                }
                else
                {
                    x[i, j] = model.NewIntVar(j * busSchedule.timeInterval[i], (j + 1) * busSchedule.timeInterval[i], $"x{i}_{j}");
                }
            }
        }

        // Decision variables y[i, j, k, l, m] (binary variables)
        IntVar[,,,,] y = new IntVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                {
                    for (int l = 0; l < carCount[k]; l++)
                    {
                        for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                        {
                            y[i, j, k, l, m] = model.NewBoolVar($"y{i}_{j}_{k}_{l}_{m}");
                        }
                    }
                }
            }
        }

        // Constraints for y variables
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int k = 0; k < busSchedule.timeInterval.Length; k++)
            {
                for (int l = 0; l < carCount[k]; l++)
                {
                    for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                    {
                        // situation 1: cannot transfer between two lines
                        if (busSchedule.timeRunning[i][m] == busSchedule.timeLimit || busSchedule.timeRunning[k][m] == busSchedule.timeLimit)
                        {
                            for (int j = 0; j < carCount[i]; j++)
                            {
                                model.Add(y[i, j, k, l, m] == 0);
                            }
                            continue;
                        }


                        for (int j = 0; j < carCount[i]; j++)
                        {
                            // situation 2: cannot transfer between two vehicles
                            if (i != k)
                            {
                                if (j == 0)
                                {
                                    if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) || (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeRunning[i][m] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                    {
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeRunning[i][m] + M * (1 - y[i, j, k, l, m]));
                                    }
                                    else
                                    {
                                        model.Add(y[i, j, k, l, m] == 0);
                                    }
                                }
                                else
                                {
                                    if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) || (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeInterval[i] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                    {
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeInterval[i] - 1 + M * (1 - y[i, j, k, l, m]));
                                    }
                                    else
                                    {
                                        model.Add(y[i, j, k, l, m] == 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Constraints for x variables (separation constraint)
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i] - 1; j++)
            {
                model.Add(x[i, j + 1] - x[i, j] == busSchedule.timeInterval[i]);
            }
        }

        // Define the objective function (maximization)
        // int scaleFactor = 10;
        var objective = Google.OrTools.Sat.LinearExpr.Sum(
            from i in Enumerable.Range(0, busSchedule.timeInterval.Length)
            from k in Enumerable.Range(0, busSchedule.timeInterval.Length)
            from m in Enumerable.Range(0, busSchedule.timeRunning[0].Length)
            let sumJl = Google.OrTools.Sat.LinearExpr.Sum(
                from j in Enumerable.Range(0, carCount[i])
                from l in Enumerable.Range(0, carCount[k])
                where i != k
                select y[i, j, k, l, m]
            )
            select sumJl * (int)(transferPassengerFlow[k, i, m]) // * scaleFactor)
        );

        model.Maximize(objective);

        // Solve the model
        CpSolver solver = new CpSolver();
        // 设置求解时间上限
        solver.StringParameters = $"max_time_in_seconds:{busSchedule.solverTimeLimit}";
        CpSolverStatus resultStatus = solver.Solve(model);

        // Output the result
        if (resultStatus == CpSolverStatus.Optimal || resultStatus == CpSolverStatus.Feasible)
        {
            Console.WriteLine("最优解:");
            Console.WriteLine($"最大化目标函数值 = {solver.ObjectiveValue}");
            // 输出x变量的值
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int j = 0; j < carCount[i]; j++)
                {
                    Console.WriteLine($"x{i}{j} = {solver.Value(x[i, j])}");
                }
            }
        }
        else
        {
            Console.WriteLine("无法找到最优解。");
        }

        return solver.ObjectiveValue;
    }

    private static void OptimizationCP_SAT_Model3(BusSchedule busSchedule, double[,,] transferPassengerFlow, double p)
    {
        // Create the CP-SAT model
        CpModel model = new CpModel();

        int M = 100000;
        int[] carCount = new int[busSchedule.timeInterval.Length];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            carCount[i] = (int)Math.Ceiling((double)busSchedule.totalTimeInterval / (double)busSchedule.timeInterval[i]); //向上取整
        }

        // Decision variables x[i, j] (integer variables)
        IntVar[,] x = new IntVar[busSchedule.timeInterval.Length, carCount.Max()];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                if ((j + 1) * busSchedule.timeInterval[i] >= busSchedule.totalTimeInterval)
                {
                    x[i, j] = model.NewIntVar(j * busSchedule.timeInterval[i], busSchedule.totalTimeInterval, $"x{i}_{j}");
                }
                else
                {
                    x[i, j] = model.NewIntVar(j * busSchedule.timeInterval[i], (j + 1) * busSchedule.timeInterval[i], $"x{i}_{j}");
                }
            }
        }

        // Decision variables y[i, j, k, l, m] (binary variables)
        IntVar[,,,,] y = new IntVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
        IntVar[,,,,] w = new IntVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
        IntVar[,,,,] u = new IntVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                {
                    for (int l = 0; l < carCount[k]; l++)
                    {
                        for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                        {
                            y[i, j, k, l, m] = model.NewBoolVar($"y{i}_{j}_{k}_{l}_{m}");
                            w[i, j, k, l, m] = model.NewIntVar(-2 * busSchedule.totalTimeInterval, 2 * busSchedule.totalTimeInterval, $"w{i}_{j}_{k}_{l}_{m}");

                            u[i, j, k, l, m] = model.NewIntVar(-2 * busSchedule.totalTimeInterval, 2 * busSchedule.totalTimeInterval, $"u{i}_{j}_{k}_{l}_{m}");

                            if (i != k)
                            {
                                model.Add(u[i, j, k, l, m] == w[i, j, k, l, m]).OnlyEnforceIf((ILiteral)y[i, j, k, l, m]);
                                model.Add(u[i, j, k, l, m] == 0).OnlyEnforceIf(((ILiteral)y[i, j, k, l, m]).Not());
                            }
                            else
                            {
                                model.Add(u[i, j, k, l, m] == 0);
                            }
                        }
                    }
                }
            }
        }

        // Constraints for y variables
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int k = 0; k < busSchedule.timeInterval.Length; k++)
            {
                for (int l = 0; l < carCount[k]; l++)
                {
                    for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                    {
                        // situation 1: cannot transfer between two lines
                        if (busSchedule.timeRunning[i][m] == busSchedule.timeLimit || busSchedule.timeRunning[k][m] == busSchedule.timeLimit)
                        {
                            for (int j = 0; j < carCount[i]; j++)
                            {
                                model.Add(y[i, j, k, l, m] == 0);
                            }
                            continue;
                        }


                        for (int j = 0; j < carCount[i]; j++)
                        {
                            // situation 2: cannot transfer between two vehicles
                            if (i != k)
                            {
                                if (j == 0)
                                {
                                    if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) || (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeRunning[i][m] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                    {
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeRunning[i][m] + M * (1 - y[i, j, k, l, m]));

                                        model.Add(w[i, j, k, l, m] <= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] + M * (1 - y[i, j, k, l, m]));
                                        model.Add(w[i, j, k, l, m] >= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] - M * (1 - y[i, j, k, l, m]));
                                    }
                                    else
                                    {
                                        model.Add(y[i, j, k, l, m] == 0);
                                    }
                                }
                                else
                                {
                                    if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) || (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeInterval[i] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                    {
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeInterval[i] - 1 + M * (1 - y[i, j, k, l, m]));

                                        model.Add(w[i, j, k, l, m] <= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] + M * (1 - y[i, j, k, l, m]));
                                        model.Add(w[i, j, k, l, m] >= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] - M * (1 - y[i, j, k, l, m]));
                                    }
                                    else
                                    {
                                        model.Add(y[i, j, k, l, m] == 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Constraints for x variables (separation constraint)
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i] - 1; j++)
            {
                model.Add(x[i, j + 1] - x[i, j] == busSchedule.timeInterval[i]);
            }
        }

        var objectiveExpr = Google.OrTools.Sat.LinearExpr.Sum(
            from i in Enumerable.Range(0, busSchedule.timeInterval.Length)
            from k in Enumerable.Range(0, busSchedule.timeInterval.Length)
            from m in Enumerable.Range(0, busSchedule.timeRunning[0].Length)
            let sumJl = Google.OrTools.Sat.LinearExpr.Sum(
                from j in Enumerable.Range(0, carCount[i])
                from l in Enumerable.Range(0, carCount[k])
                where i != k
                select y[i, j, k, l, m]
            )
            select sumJl * (int)(transferPassengerFlow[k, i, m])
        );
        model.Add(objectiveExpr == (int)p);

        // Define the objective function (maximization)
        // int scaleFactor = 10;
        var objective = Google.OrTools.Sat.LinearExpr.Sum(
            from i in Enumerable.Range(0, busSchedule.timeInterval.Length)
            from k in Enumerable.Range(0, busSchedule.timeInterval.Length)
            from m in Enumerable.Range(0, busSchedule.timeRunning[0].Length)
            let sumJl = Google.OrTools.Sat.LinearExpr.Sum(
                from j in Enumerable.Range(0, carCount[i])
                from l in Enumerable.Range(0, carCount[k])
                where i != k
                select u[i, j, k, l, m]
            )
            select sumJl * (int)(transferPassengerFlow[k, i, m]) // * scaleFactor)
        );

        model.Minimize(objective);

        // Solve the model
        CpSolver solver = new CpSolver();
        // 设置求解时间上限
        solver.StringParameters = $"max_time_in_seconds:{busSchedule.solverTimeLimit}";
        CpSolverStatus resultStatus = solver.Solve(model);

        // Output the result
        if (resultStatus == CpSolverStatus.Optimal || resultStatus == CpSolverStatus.Feasible)
        {
            Console.WriteLine("最优解:");
            Console.WriteLine($"最小化目标函数值 = {solver.ObjectiveValue}");

        }
        else
        {
            Console.WriteLine("无法找到最优解。");
        }
    }

    private static void OptimizationGurobi_Model3(BusSchedule busSchedule, double[,,] transferPassengerFlow, double p)
    {
        try
        {
            GRBEnv env = new();
            GRBModel model = new(env);

            int M = 100000;
            int[] carCount = new int[busSchedule.timeInterval.Length];
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                carCount[i] = (int)Math.Ceiling((double)busSchedule.totalTimeInterval / (double)busSchedule.timeInterval[i]);
            }

            GRBVar[,] x = new GRBVar[busSchedule.timeInterval.Length, carCount.Max()];
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int j = 0; j < carCount[i]; j++)
                {
                    x[i, j] = model.AddVar(0.0, busSchedule.totalTimeInterval, 0.0, GRB.INTEGER, "x" + i + j);
                }
            }

            GRBVar[,,,,] y = new GRBVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
            GRBVar[,,,,] w = new GRBVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
            GRBVar[,,,,] u = new GRBVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int j = 0; j < carCount[i]; j++)
                {
                    for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                    {
                        for (int l = 0; l < carCount[k]; l++)
                        {
                            for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                            {
                                y[i, j, k, l, m] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, "y" + i + j + k + l + m);
                                w[i, j, k, l, m] = model.AddVar(-busSchedule.totalTimeInterval, busSchedule.totalTimeInterval, 0.0, GRB.INTEGER, "w" + i + j + k + l + m);

                                if (i != k)
                                {
                                    u[i, j, k, l, m] = model.AddVar(0, busSchedule.totalTimeInterval, transferPassengerFlow[k, i, m], GRB.INTEGER, "u" + i + j + k + l + m);
                                    model.AddQConstr(u[i, j, k, l, m] == w[i, j, k, l, m] * y[i, j, k, l, m], "u" + i + j + k + l + m);
                                }
                                else
                                {
                                    u[i, j, k, l, m] = model.AddVar(0.0, 0.0, 0.0, GRB.INTEGER, "u" + i + j + k + l + m);
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                {
                    for (int l = 0; l < carCount[k]; l++)
                    {
                        for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                        {
                            if (busSchedule.timeRunning[i][m] == busSchedule.timeLimit || busSchedule.timeRunning[k][m] == busSchedule.timeLimit)
                            {
                                for (int j = 0; j < carCount[i]; j++)
                                {
                                    model.AddConstr(y[i, j, k, l, m] == 0, "");
                                }
                                continue;
                            }

                            for (int j = 0; j < carCount[i]; j++)
                            {
                                if (i != k)
                                {
                                    if (j == 0)
                                    {
                                        if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) ||
                                              (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeRunning[i][m] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                        {
                                            model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]), "");
                                            model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeRunning[i][m] + M * (1 - y[i, j, k, l, m]), "");

                                            model.AddConstr(w[i, j, k, l, m] <= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] + M * (1 - y[i, j, k, l, m]), "");
                                            model.AddConstr(w[i, j, k, l, m] >= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] - M * (1 - y[i, j, k, l, m]), "");
                                        }
                                        else
                                        {
                                            model.AddConstr(y[i, j, k, l, m] == 0, "");
                                        }
                                    }
                                    else
                                    {
                                        if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) ||
                                              (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeInterval[i] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                        {
                                            model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]), "");
                                            model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeInterval[i] - 1 + M * (1 - y[i, j, k, l, m]), "");

                                            model.AddConstr(w[i, j, k, l, m] <= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] + M * (1 - y[i, j, k, l, m]), "");
                                            model.AddConstr(w[i, j, k, l, m] >= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] - M * (1 - y[i, j, k, l, m]), "");
                                        }
                                        else
                                        {
                                            model.AddConstr(y[i, j, k, l, m] == 0, "");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int j = 0; j < carCount[i] - 1; j++)
                {
                    model.AddConstr(x[i, j + 1] - x[i, j] == busSchedule.timeInterval[i], "");
                }
            }

            // Create the objective expression for Gurobi
            GRBLinExpr objectiveExpr = new GRBLinExpr();
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                {
                    for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                    {
                        for (int j = 0; j < carCount[i]; j++)
                        {
                            for (int l = 0; l < carCount[k]; l++)
                            {
                                if (i != k)
                                {
                                    objectiveExpr.AddTerm(transferPassengerFlow[k, i, m], y[i, j, k, l, m]);
                                }
                            }
                        }
                    }
                }
            }
            // Add the objective expression as a constraint or objective
            model.AddConstr(objectiveExpr == p, "ObjectiveConstraint");


            // Set time limit
            model.Parameters.TimeLimit = busSchedule.solverTimeLimit;
            model.Optimize();

            // Output
            if (model.Status == GRB.Status.OPTIMAL)
            {
                Console.WriteLine("Optimal Solution Found");
                Console.WriteLine($"Objective Value = {model.ObjVal}");
                //// Print the value of x[i, j]
                //for (int i = 0; i < busSchedule.timeInterval.Length; i++)
                //{
                //    for (int j = 0; j < carCount[i]; j++)
                //    {
                //        // Get the value of x[i, j] and print it
                //        double xValue = x[i, j].X;
                //        Console.WriteLine($"x[{i}, {j}] = {xValue}");
                //    }
                //}
                //for (int i = 0; i < busSchedule.timeInterval.Length; i++)
                //{
                //    for (int j = 0; j < carCount[i]; j++)
                //    {
                //        for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                //        {
                //            for (int l = 0; l < carCount[k]; l++)
                //            {
                //                for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                //                {
                //                    // Get the value of w[i, j, k, l, m] and print it
                //                    double wValue = w[i, j, k, l, m].X;
                //                    Console.WriteLine($"w[{i}, {j}, {k}, {l}, {m}] = {wValue}");
                //                }
                //            }
                //        }
                //    }
                //}
            }
            else
            {
                Console.WriteLine("No Optimal Solution Found");
            }

            model.Dispose();
            env.Dispose();
        }
        catch
        {


        }
    }

    private static void OptimizationGurobi_Model4(BusSchedule busSchedule, double[,,] transferPassengerFlow, double p)
    {
        try
        {
            GRBEnv env = new();
            GRBModel model = new(env);

            int M = 100000;
            int[] carCount = new int[busSchedule.timeInterval.Length];
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                carCount[i] = (int)Math.Ceiling((double)busSchedule.totalTimeInterval / (double)busSchedule.timeInterval[i]);
            }

            GRBVar[,] x = new GRBVar[busSchedule.timeInterval.Length, carCount.Max()];
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int j = 0; j < carCount[i]; j++)
                {
                    x[i, j] = model.AddVar(0.0, busSchedule.totalTimeInterval, 0.0, GRB.INTEGER, "x" + i + j);
                }
            }

            GRBVar W = model.AddVar(-2 * busSchedule.totalTimeInterval, 2 * busSchedule.totalTimeInterval, 1.0, GRB.INTEGER, "W");
            GRBVar[,,,,] y = new GRBVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
            GRBVar[,,,,] w = new GRBVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int j = 0; j < carCount[i]; j++)
                {
                    for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                    {
                        for (int l = 0; l < carCount[k]; l++)
                        {
                            for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                            {
                                y[i, j, k, l, m] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, "y" + i + j + k + l + m);
                                w[i, j, k, l, m] = model.AddVar(-2 * busSchedule.totalTimeInterval, 2 * busSchedule.totalTimeInterval, 0.0, GRB.INTEGER, "w" + i + j + k + l + m);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                {
                    for (int l = 0; l < carCount[k]; l++)
                    {
                        for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                        {
                            if (busSchedule.timeRunning[i][m] == busSchedule.timeLimit || busSchedule.timeRunning[k][m] == busSchedule.timeLimit)
                            {
                                for (int j = 0; j < carCount[i]; j++)
                                {
                                    model.AddConstr(y[i, j, k, l, m] == 0, "");
                                }
                                continue;
                            }

                            for (int j = 0; j < carCount[i]; j++)
                            {
                                if (i != k)
                                {
                                    if (j == 0)
                                    {
                                        if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) ||
                                              (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeRunning[i][m] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                        {
                                            model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]), "");
                                            model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeRunning[i][m] + M * (1 - y[i, j, k, l, m]), "");

                                            model.AddConstr(w[i, j, k, l, m] <= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] + M * (1 - y[i, j, k, l, m]), "");
                                            model.AddConstr(w[i, j, k, l, m] >= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] - M * (1 - y[i, j, k, l, m]), "");
                                        }
                                        else
                                        {
                                            model.AddConstr(y[i, j, k, l, m] == 0, "");
                                        }
                                    }
                                    else
                                    {
                                        if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) ||
                                              (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeInterval[i] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                        {
                                            model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]), "");
                                            model.AddConstr(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeInterval[i] - 1 + M * (1 - y[i, j, k, l, m]), "");

                                            model.AddConstr(w[i, j, k, l, m] <= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] + M * (1 - y[i, j, k, l, m]), "");
                                            model.AddConstr(w[i, j, k, l, m] >= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] - M * (1 - y[i, j, k, l, m]), "");
                                        }
                                        else
                                        {
                                            model.AddConstr(y[i, j, k, l, m] == 0, "");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int j = 0; j < carCount[i] - 1; j++)
                {
                    model.AddConstr(x[i, j + 1] - x[i, j] == busSchedule.timeInterval[i], "");
                }
            }

            // Create the objective expression for Gurobi
            GRBLinExpr objectiveExpr = new GRBLinExpr();
            for (int i = 0; i < busSchedule.timeInterval.Length; i++)
            {
                for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                {
                    for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                    {
                        for (int j = 0; j < carCount[i]; j++)
                        {
                            for (int l = 0; l < carCount[k]; l++)
                            {
                                if (i != k)
                                {
                                    objectiveExpr.AddTerm(transferPassengerFlow[k, i, m], y[i, j, k, l, m]);
                                    model.AddQConstr(y[i, j, k, l, m] * (w[i, j, k, l, m] - W) <= 0, "");
                                }
                            }
                        }
                    }
                }
            }
            // Add the objective expression as a constraint or objective
            model.AddConstr(objectiveExpr == p, "ObjectiveConstraint");


            // Set time limit
            model.Parameters.TimeLimit = busSchedule.solverTimeLimit;
            model.Optimize();

            // Output
            if (model.Status == GRB.Status.OPTIMAL)
            {
                Console.WriteLine("Optimal Solution Found");
                Console.WriteLine($"Objective Value = {model.ObjVal}");
                // Print the value of x[i, j]
                for (int i = 0; i < busSchedule.timeInterval.Length; i++)
                {
                    for (int j = 0; j < carCount[i]; j++)
                    {
                        // Get the value of x[i, j] and print it
                        double xValue = x[i, j].X;
                        Console.WriteLine($"x[{i}, {j}] = {xValue}");
                    }
                }
                for (int i = 0; i < busSchedule.timeInterval.Length; i++)
                {
                    for (int j = 0; j < carCount[i]; j++)
                    {
                        for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                        {
                            for (int l = 0; l < carCount[k]; l++)
                            {
                                for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                                {
                                    // Get the value of w[i, j, k, l, m] and print it
                                    double wValue = w[i, j, k, l, m].X;
                                    Console.WriteLine($"w[{i}, {j}, {k}, {l}, {m}] = {wValue}");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No Optimal Solution Found");
            }

            model.Dispose();
            env.Dispose();
        }
        catch
        {


        }
    }

    private static void OptimizationCP_SAT_Model4(BusSchedule busSchedule, double[,,] transferPassengerFlow, double p)
    {
        // Create the CP-SAT model
        CpModel model = new CpModel();

        int M = 100000;
        int[] carCount = new int[busSchedule.timeInterval.Length];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            carCount[i] = (int)Math.Ceiling((double)busSchedule.totalTimeInterval / (double)busSchedule.timeInterval[i]); //向上取整
        }

        // Decision variables x[i, j] (integer variables)
        IntVar[,] x = new IntVar[busSchedule.timeInterval.Length, carCount.Max()];
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                if ((j + 1) * busSchedule.timeInterval[i] >= busSchedule.totalTimeInterval)
                {
                    x[i, j] = model.NewIntVar(j * busSchedule.timeInterval[i], busSchedule.totalTimeInterval, $"x{i}_{j}");
                }
                else
                {
                    x[i, j] = model.NewIntVar(j * busSchedule.timeInterval[i], (j + 1) * busSchedule.timeInterval[i], $"x{i}_{j}");
                }
            }
        }

        // Decision variables y[i, j, k, l, m] (binary variables)
        IntVar[,,,,] y = new IntVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
        IntVar[,,,,] w = new IntVar[busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeInterval.Length, carCount.Max(), busSchedule.timeRunning[0].Length];
        IntVar W = model.NewIntVar(0, 2 * busSchedule.totalTimeInterval, $"w");
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i]; j++)
            {
                for (int k = 0; k < busSchedule.timeInterval.Length; k++)
                {
                    for (int l = 0; l < carCount[k]; l++)
                    {
                        for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                        {
                            y[i, j, k, l, m] = model.NewBoolVar($"y{i}_{j}_{k}_{l}_{m}");
                            w[i, j, k, l, m] = model.NewIntVar(-2 * busSchedule.totalTimeInterval, 2 * busSchedule.totalTimeInterval, $"w{i}_{j}_{k}_{l}_{m}");

                            if (i != k)
                            {
                                model.Add(0 >= w[i, j, k, l, m] - W - M * (1 - y[i, j, k, l, m]));
                            }
                        }
                    }
                }
            }
        }

        // Constraints for y variables
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int k = 0; k < busSchedule.timeInterval.Length; k++)
            {
                for (int l = 0; l < carCount[k]; l++)
                {
                    for (int m = 0; m < busSchedule.timeRunning[0].Length; m++)
                    {
                        // situation 1: cannot transfer between two lines
                        if (busSchedule.timeRunning[i][m] == busSchedule.timeLimit || busSchedule.timeRunning[k][m] == busSchedule.timeLimit)
                        {
                            for (int j = 0; j < carCount[i]; j++)
                            {
                                model.Add(y[i, j, k, l, m] == 0);
                            }
                            continue;
                        }


                        for (int j = 0; j < carCount[i]; j++)
                        {
                            // situation 2: cannot transfer between two vehicles
                            if (i != k)
                            {
                                if (j == 0)
                                {
                                    if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) || (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeRunning[i][m] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                    {
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeRunning[i][m] + M * (1 - y[i, j, k, l, m]));

                                        model.Add(w[i, j, k, l, m] <= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] + M * (1 - y[i, j, k, l, m]));
                                        model.Add(w[i, j, k, l, m] >= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] - M * (1 - y[i, j, k, l, m]));
                                    }
                                    else
                                    {
                                        model.Add(y[i, j, k, l, m] == 0);
                                    }
                                }
                                else
                                {
                                    if (!((busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + l * busSchedule.timeInterval[k] > (j + 1) * busSchedule.timeInterval[i] - 1 + busSchedule.timeRunning[i][m]) || (busSchedule.timeRunning[k][m] + busSchedule.timeTransfer[m] + (l + 1) * busSchedule.timeInterval[k] - 1 + busSchedule.timeInterval[i] - 1 < j * busSchedule.timeInterval[i] + busSchedule.timeRunning[i][m])))
                                    {
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] >= -M * (1 - y[i, j, k, l, m]));
                                        model.Add(x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] <= busSchedule.timeInterval[i] - 1 + M * (1 - y[i, j, k, l, m]));

                                        model.Add(w[i, j, k, l, m] <= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] + M * (1 - y[i, j, k, l, m]));
                                        model.Add(w[i, j, k, l, m] >= x[i, j] + busSchedule.timeRunning[i][m] - x[k, l] - busSchedule.timeRunning[k][m] - busSchedule.timeTransfer[m] - M * (1 - y[i, j, k, l, m]));
                                    }
                                    else
                                    {
                                        model.Add(y[i, j, k, l, m] == 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Constraints for x variables (separation constraint)
        for (int i = 0; i < busSchedule.timeInterval.Length; i++)
        {
            for (int j = 0; j < carCount[i] - 1; j++)
            {
                model.Add(x[i, j + 1] - x[i, j] == busSchedule.timeInterval[i]);
            }
        }

        var objectiveExpr = Google.OrTools.Sat.LinearExpr.Sum(
            from i in Enumerable.Range(0, busSchedule.timeInterval.Length)
            from k in Enumerable.Range(0, busSchedule.timeInterval.Length)
            from m in Enumerable.Range(0, busSchedule.timeRunning[0].Length)
            let sumJl = Google.OrTools.Sat.LinearExpr.Sum(
                from j in Enumerable.Range(0, carCount[i])
                from l in Enumerable.Range(0, carCount[k])
                where i != k
                select y[i, j, k, l, m]
            )
            select sumJl * (int)(transferPassengerFlow[k, i, m])
        );
        model.Add(objectiveExpr == (int)p);

        // Define the objective function (maximization)
        // int scaleFactor = 10;
        var objective = W;

        model.Minimize(objective);

        // Solve the model
        CpSolver solver = new CpSolver();
        // 设置求解时间上限
        solver.StringParameters = $"max_time_in_seconds:{busSchedule.solverTimeLimit}";
        CpSolverStatus resultStatus = solver.Solve(model);

        // Output the result
        if (resultStatus == CpSolverStatus.Optimal || resultStatus == CpSolverStatus.Feasible)
        {
            Console.WriteLine("最优解:");
            Console.WriteLine($"最小化目标函数值 = {solver.ObjectiveValue}");

        }
        else
        {
            Console.WriteLine("无法找到最优解。");
        }
    }
}


class BusSchedule
{
    public int[] timeInterval;
    public int[][] timeRunning;
    public int[] timeTransfer;
    public int totalTimeInterval;
    public int timeLimit;
    public double solverTimeLimit;

    public BusSchedule(int[] timeInterval, int[][] timeRunning, int[] timeTransfer, int totalTimeInterval, int timeLimit, double solverTimeLimit)
    {
        this.timeInterval = timeInterval;
        this.timeRunning = timeRunning;
        this.timeTransfer = timeTransfer;
        this.totalTimeInterval = totalTimeInterval;
        this.timeLimit = timeLimit;
        this.solverTimeLimit = solverTimeLimit;
    }
}

