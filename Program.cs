using System;
using Gurobi;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Google.OrTools.Glop;
using Google.OrTools.LinearSolver;

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

        BusSchedule busSchedule = new(timeInterval, timeRunning, timeTransfer, totalTimeInterval, timeLimit);

        Optimization(busSchedule);
        OptimizationOrtools(busSchedule);
    }



    private static void OptimizationOrtools(BusSchedule busSchedule)
    {
        // Create the solver
        Solver solver = Solver.CreateSolver("CBC_MIXED_INTEGER_PROGRAMMING");

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

                        Constraint constraint = solver.MakeConstraint(double.NegativeInfinity, 10, $"constraint1:{i} {k} {l} {m}");

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
        }
        catch
        {

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

    public BusSchedule(int[] timeInterval, int[][] timeRunning, int[] timeTransfer, int totalTimeInterval, int timeLimit)
    {
        this.timeInterval = timeInterval;
        this.timeRunning = timeRunning;
        this.timeTransfer = timeTransfer;
        this.totalTimeInterval = totalTimeInterval;
        this.timeLimit = timeLimit;
    }
}

